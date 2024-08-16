using Newtonsoft.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using SearchNavigate.Core.Domain.Models;

namespace SearchNavigate.Infrastructure.Persistence.Context;
public class DataSeeding
{
   public class View
   {
      public record Properties
      {
         public Guid ProductId { get; set; }
         public string ProductName { get; set; }
      }
      public record Context
      {
         public int Source { get; set; }
      }
      public record User
      {
         public Guid Id { get; set; }
         public string UserName { get; set; }
      }
      public string Event { get; set; }
      public Guid MessageId { get; set; }
      public User user { get; set; }
      public Properties properties { get; set; }
      public Context context { get; set; }
      public DateTime DateTime { get; set; }
   }

   /// <summary>
   /// Generates a list of users with arbitrary first names, last names, and unique IDs.
   /// The user's full name is then concatenated to form the user name.
   /// </summary>
   /// <returns>A list of users.</returns>
   private static List<User> GetUsers()
   {
      List<User> users = new Faker<User>("tr")
         .RuleFor(i => i.Id, i => Guid.NewGuid())
         .RuleFor(i => i.FirstName, i => i.Person.FirstName)
         .RuleFor(i => i.LastName, i => i.Person.LastName)
         .Generate(1000);

      users = users
         .Select(i =>
         {
            i.UserName = i.FirstName + "_" + i.LastName;
            return i;
         })
         .ToList();
      return users;
   }
   /// <summary>
   /// Generates fake(but looks realistic) data and feeds to database.
   /// </summary>
   /// <param name="Db_Connection">The connection string for the database.</param>
   /// <param name="jsonFilePath">The path to the JSON file to write down disk for reading ViewProducer App</param>
   public async Task SeedAsync(string Db_Connection, string jsonFilePath)
   {
      var dbContextBuilder = new DbContextOptionsBuilder();
      dbContextBuilder.UseNpgsql(Db_Connection);

      var context = new SearchNavigateDbContext(dbContextBuilder.Options);

      if (context.DbSetUser.Any()) // Check if there is any data, if so don't continue.
      {
         await Task.CompletedTask;
         return;
      }

      var users = GetUsers();
      var categories = GetCategories();
      var products = Shuffle(GetProducts(categories));
      var (orderItems, orders) = GetOrders(users, products);
      var viewHistory = GenerateViewHistory(jsonFilePath, users, products);

      await context.DbSetUser.AddRangeAsync(users);
      await context.DbSetCategory.AddRangeAsync(categories);
      await context.DbSetProduct.AddRangeAsync(products);
      await context.DbSetOrder.AddRangeAsync(orders);
      await context.DbSetOrderItems.AddRangeAsync(orderItems);
      await context.DbSetViewHistory.AddRangeAsync(viewHistory);
      await context.SaveChangesAsync();
   }

   private List<UserViewHistory> GenerateViewHistory(string jsonFilePath, List<User> users, List<Product> products)
   {
      // Generates view history for each user in a random number of times and random times/order
      List<Guid> userViews = GenerateProductViews();

      // Generate random number of view history per user but write to disk and don't feed to database
      using (StreamWriter sw = new StreamWriter(jsonFilePath))
      {
         // Generated view history is written to disk
         foreach (var user_id in userViews)
         {
            User nextUser = users.FirstOrDefault(i => i.Id == user_id);
            var rndProduct = PickRandomly(products);
            var jsonString = JsonConvert.SerializeObject(new View()
            {
               Event = "ProductView",
               MessageId = Guid.NewGuid(),
               user = new View.User()
               {
                  Id = nextUser.Id,
                  UserName = nextUser.UserName
               },
               properties = new View.Properties()
               {
                  ProductId = rndProduct.Id,
                  ProductName = rndProduct.ProductName
               },
               context = new View.Context() { Source = PickRandomFromEnum<ViewSourse>() },
               DateTime = DateTime.Now,
            }, Formatting.None);
            sw.WriteLine(jsonString);

         }
      }


      List<UserViewHistory> userViewHistories = new();
      userViews = GenerateProductViews();
      // Generate random number of view history per user but don't write to disk and feed to database
      // feeding database is done after returning from this function.
      foreach (var user_id in userViews)
      {
         User nextUser = users.FirstOrDefault(i => i.Id == user_id);
         var rndProduct = PickRandomly(products);

         userViewHistories.Add(
            new UserViewHistory()
            {
               UserId = nextUser.Id,
               ProductId = rndProduct.Id,
               ViewDate = DateTime.Today.AddDays(rnd.Next(-50, 0))
            });
      }

      return userViewHistories;


      // Generates random number of view ranges between from 15 to 3 per user
      List<Guid> GenerateProductViews()
      {
         List<Guid> views = new();
         List<int> ints = GetCurvedNumbers(15, 3, null, i => (int)Math.Pow(i, 2));

         foreach (var user in users)
         {
            int viewCount = PickRandomly(ints);
            for (int i = 0; i < viewCount; i++)
               views.Add(user.Id);
         }
         return Shuffle(views);
      }
   }

   private (List<OrderItems>, List<Order>) GetOrders(List<User> users, List<Product> products)
   {
      List<Order> orders = new();
      List<OrderItems> orderItems = new();
      var orderCount = GetCurvedNumbers(6, 0); // An user could order at most 6 times and at least 0 times.
      var basketCount = GetCurvedNumbers(4, 1); // An order could have at most 4 items and at least 1 item.

      foreach (User user in users) // Loop through generated random users.
      {
         int orderQuantity = PickRandomly(orderCount); // Pick random order quantity per user.
         for (int i = 0; i < orderQuantity; i++) // Loop through orders that is going to be generated(). 
         {
            int basketQuantity = PickRandomly(basketCount); // Determine the number of items in the basket
            Order order = new()
            {
               Id = Guid.NewGuid(),
               OrderedBy = user.Id,
            };
            for (int j = 0; j < basketQuantity; j++)
            {
               Product productId = PickRandomly(products); // Pick random product from the products list.
               orderItems.Add(new OrderItems() // Add the all products into basket. 
               {
                  Id = Guid.NewGuid(),
                  OrderId = order.Id,
                  ProductId = productId.Id,
                  Quantity = 1,
               });
            }
            orders.Add(order);
         }
      }
      return (orderItems, orders);
   }

   /// <summary>
   /// Retrieves a list of categories available in the codebase.
   /// </summary>
   /// <returns>A list of Categories.</returns>
   private List<Category> GetCategories()
   {
      return categories_products.Keys
        .Select(i => new Category()
        {
           Id = Guid.NewGuid(),
           CategoryName = i
        })
        .ToList();
   }

   /// <summary>
   /// Populate the list of products based on the given list of categories.
   /// </summary>
   /// <param name="categories">The list of categories to populate.</param>
   /// <returns>A list of products.</returns>
   private static List<Product> GetProducts(List<Category> categories)
   {
      List<Product> products = new();
      foreach (var category in categories)
         for (int i = 0; i < 20; i++)
            products.Add(
               new Product()
               {
                  Id = Guid.NewGuid(),
                  CategoryId = category.Id,
                  ProductName = categories_products[category.CategoryName][i]
               });
      return products;
   }

   /// <summary>
   /// Generates a sequance of numbers increasing/decreasing order based on the given range that looks like more real world data.
   /// Not lineer but exponential.
   /// </summary>
   /// <param name="max">The maximum value for the curved numbers.</param>
   /// <param name="min">The minimum value for the curved numbers.</param>
   /// <param name="repeatTimes">An optional function that determines how many times a number will be genenerated.</param>
   /// <param name="repetadNumber">An optional function that determines what the next number will be from start point up to end</param>
   /// <returns>A list of curved numbers.</returns>
   private List<int> GetCurvedNumbers(int max, int min, Func<int, int> repeatTimes = null, Func<int, int> repetadNumber = null)
   {
      List<int> numbers = new();

      repeatTimes ??= i => (int)Math.Pow(i, 2);
      repetadNumber ??= i => i < 0 ? 0 : i;
      for (int j = max, i = 1; j >= min; j--, i++)
         numbers.AddRange(Enumerable.Repeat(repetadNumber(j), repeatTimes(i)));
      // if (j < 0)
      //    numbers.AddRange(Enumerable.Repeat(0, repeatTimes(i)));
      // else

      return Shuffle(numbers);
   }

   private int PickRandomFromEnum<T>() where T : Enum
   {
      var values = Enum.GetValues(typeof(T));
      var result = values.GetValue(rnd.Next(0, values.Length));
      return Convert.ToInt32(result);
   }

   private static Random rnd = new();
   private static T PickRandomly<T>(IList<T> list)
   {
      return list[rnd.Next(0, list.Count)];
   }

   /// <summary>
   /// Shuffles the elements in the given list by using the Fisher-Yates algorithm.
   /// </summary>
   /// <typeparam name="T">The type of elements in the list.</typeparam>
   /// <param name="list">The list to shuffle.</param>
   /// <returns>Returns clone of the given list as shuffled.</returns>
   private static int PickRandomly(object[] arr)
   {
      return Convert.ToInt32(arr[rnd.Next(0, arr.Length)]);
   }

   /// <summary>
   /// Shuffles the elements in the given list by using the Fisher-Yates algorithm.
   /// </summary>
   /// <typeparam name="T">The type of elements in the list.</typeparam>
   /// <param name="list">The list to shuffle.</param>
   /// <returns>Returns clone of the given list as shuffled.</returns>
   public static List<T> Shuffle<T>(IList<T> list)
   {
      List<T> clone = new(list);
      int n = list.Count;
      while (n > 1)
      {
         n--;
         int k = rnd.Next(n + 1);
         T value = clone[k];
         clone[k] = clone[n];
         clone[n] = value;
      }
      return clone;
   }

   private static Dictionary<string, List<string>> categories_products = new() {
      {"Electronics", new List<string>() {
            "Samsung Galaxy S22 Smartphone",
            "Dell Vostro 3520 Computer",
            "Apple iPad Air 5th Gen",
            "Apple Watch Series 7",
            "Sony WH-1000XM4 Headphones",
            "JBL Flip 5 Bluetooth Speaker",
            "Canon EOS R6 Mirrorless Camera",
            "Amazon Echo Dot 4th Gen",
            "DJI Mavic Air 2 Drone",
            "PlayStation 5 Console",
            "Oculus Quest 2 VR Headset",
            "Seagate Backup Plus Hub 8TB",
            "SanDisk Ultra Fit 256GB USB Drive",
            "ASUS ProArt PA278QV Monitor",
            "HP OfficeJet Pro 9025e Printer",
            "BenQ TK700STi 4K HDR Gaming Projector",
            "Anker PowerLine+ II USB-C Cable",
            "Mophie Powerstation PD XL",
            "Blue Yeti Nano USB Microphone",
            "Samsung QN900A Neo QLED 8K TV"
         }
      },
      {"Fashion", new List<string>() {
            "Levi's 501 Original Fit Jeans",
            "Zara Floral Print Dress",
            "The North Face Thermoball Eco Jacket",
            "Nike Air Force 1 '07 Sneakers",
            "Coach Ava Crossbody Bag",
            "Ray-Ban Wayfarer Sunglasses",
            "Timex Weekender Watch",
            "Athleta Salutation Stash Pocket II 7/8 Tight",
            "Speedo Women's Solid Endurance+ Flyback Training Swimsuit",
            "Adidas Originals Trefoil Beanie",
            "Burberry Giant Icon Check Scarf",
            "Fossil Ingram Leather Belt",
            "Pandora Moments Snake Chain Bracelet",
            "Badgley Mischka Kiara Pumps",
            "Lululemon Align Pant 25",
            "Tory Burch Miller Cloud Flip Flop",
            "Kate Spade New York Margaux Medium Satchel",
            "Oakley Flak 2.0 XL Sunglasses",
            "Fossil Nate Chronograph Leather Watch",
            "Herschel Supply Co. Seventeen Hip Pack"
         }
      },
      {
         "Home and Kitchen", new List<string>() {
            "Calphalon Premier Space Saving Hard-Anodized Nonstick 10-Piece Set",
            "KitchenAid Artisan Series 5-Quart Tilt-Head Stand Mixer",
            "Lenox Butterfly Meadow 18-Piece Dinnerware Set",
            "Victorinox Fibrox Pro 8-Inch Chef's Knife",
            "Rubbermaid Brilliance Food Storage Containers",
            "OXO Good Grips 15-Piece Everyday Kitchen Utensil Set",
            "Ninja Specialty Coffee Maker with Glass Carafe",
            "Vitamix Explorian Blender",
            "Breville Bit More 2-Slice Toaster",
            "Cuisinart 14-Cup Food Processor",
            "Utopia Kitchen 12-Pack Dish Towels",
            "Mainstays Solid Tablecloth",
            "Rivet Emerly Mid-Century Modern Sofa",
            "Brightech Ambience Pro Solar Outdoor String Lights",
            "Safavieh Adirondack Collection Area Rug",
            "Zinus Shalini 14 Inch Metal Platform Bed Frame",
            "Nestwell 400 Thread Count Sateen Sheet Set",
            "Exclusive Home Curtains Sateen Twill Curtain Panel Pair",
            "Rivet Emerly Mid-Century Modern Sofa",
            "Mainstays Solid Tablecloth",
         }
      },
      {
         "Health and Beauty", new List<string>() {
            "Neutrogena Rapid Wrinkle Repair Retinol Oil",
            "Maybelline New York Instant Age Rewind Eraser Dark Circles Treatment Concealer",
            "Pantene Pro-V Miracles Repair & Protect Shampoo",
            "Chanel Coco Mademoiselle Eau de Parfum Spray",
            "Sally Hansen Miracle Gel Nail Polish",
            "Crest 3D White Professional Effects Whitestrips",
            "Nature Made Men's Multivitamin Softgels",
            "Bowflex SelectTech 552 Adjustable Dumbbell",
            "Philips Sonicare ProtectiveClean 4100 Electric Toothbrush",
            "Gillette Fusion5 ProGlide Men's Razor",
            "Utopia Towels Luxury Bath Towels",
            "Revlon One-Step Hair Dryer And Volumizer Hot Air Brush",
            "Neutrogena Ultra Sheer Dry-Touch Sunscreen",
            "Aveeno Daily Moisturizing Lotion",
            "Garnier SkinActive Soothing Facial Mist with Rose Water",
            "Old Spice High Endurance Deodorant",
            "Dr. Scholl's Corn Removers",
            "Gillette Fusion5 ProGlide Men's Razor",
            "Fitbit Charge 5 Advanced Fitness & Health Tracker",
            "NOW Solutions Peppermint Essential Oil"
         }
      },
      {
         "Baby Products", new List<string>() {
            "Pampers Swaddlers Disposable Baby Diapers",
            "Huggies Natural Care Baby Wipes",
            "Carter's Baby Boys' 6-Piece Bodysuit & Pant Set",
            "Graco Modes Nest Travel System",
            "Britax One4Life ClickTight All-in-One Car Seat",
            "Infant Optics DXR-8 Video Baby Monitor",
            "Philips Avent Soothie Pacifier",
            "Dr. Brown's Natural Flow Options+ Wide-Neck Bottle",
            "Vulli Sophie the Giraffe Teether",
            "Gerber Organic 2nd Foods Baby Food",
            "Delta Children Emery 4-in-1 Convertible Crib",
            "Graco Blossom 6-in-1 Convertible High Chair",
            "Aden + Anais Classic Muslin Swaddle Blankets",
            "Munchkin Float and Play Bubbles Bath Toy",
            "Boppy Original Nursing Pillow and Positioner",
            "Regalo Easy Step Walk Thru Gate",
            "Graco Pack 'n Play On The Go Playard",
            "Baby Bjorn Baby Carrier One",
            "Dr. Seuss's ABC Book",
            "Britax Advocate ClickTight Anti-Rebound Bar Convertible Car Seat"
         }
      },
      {
         "Sports and Outdoors", new List<string>(){
            "Schwinn Phocus 1600 Road Bicycle",
            "Coleman Sundome 2-Person Tent",
            "Marmot Trestles 30 Degree Sleeping Bag",
            "Columbia Newton Ridge Plus Waterproof Hiking Boot",
            "Under Armour Men's Tech 2.0 Short-Sleeve T-Shirt",
            "Fitbit Charge 5 Fitness Tracker",
            "Manduka PRO Yoga Mat",
            "PowerBlock Sport 24 Pound Dumbbell Set",
            "Adidas MLS Glider Soccer Ball",
            "Spalding NBA Street Basketball",
            "Wilson Federer Tennis Racket",
            "Plusinno Telescopic Fishing Rod and Reel Combo",
            "Thule Chariot Cross 2 Stroller",
            "Hydro Flask 32 oz Wide Mouth Water Bottle",
            "Coleman 54-Quart Wheeled Cooler",
            "Keter Rio 3-Piece Patio Bistro Set",
            "Weber Q1200 Liquid Propane Grill",
            "Wavestorm 8' Classic Surfboard",
            "Black Diamond Half Dome Climbing Helmet",
            "Speedo Mens Surfwalker Pro 3.0 Water Shoe"
         }
      },
      {
         "Pet Supplies", new List<string>(){
            "Purina ONE SmartBlend Natural Dry Dog Food",
            "KONG Classic Dog Toy",
            "Furminator Deshedding Tool for Dogs",
            "Midwest Homes for Pets iCrate Starter Kit",
            "PetSafe EasyWalk Harness",
            "Frisco Training and Potty Pads",
            "Frisco Colorblocked Dog Hoodie",
            "Aqueon LED MiniBow Aquarium Kit",
            "Sherpa Original Deluxe Airline Approved Pet Carrier",
            "Zesty Paws Omega-3 Alaskan Fish Oil Chews",
            "Arm & Hammer Clump & Seal Litter",
            "Purina Friskies Wet Cat Food",
            "Furminator Deshedding Tool for Cats",
            "Petmate Deluxe Pearl Pet Pen",
            "Frisco Plush Squeaky Elephant Dog Toy",
            "Petmate Compass Plastic Kennel",
            "Paw5 Wooly Mat Snuffle Mat",
            "Frisco Stainless Steel Dog Bowl",
            "Petsafe Smart Feed Automatic Dog and Cat Feeder",
            "Frisco Plush Squeaky Elephant Dog Toy"
         }
      },
      {
         "Automotive", new List<string>(){
            "Armor All Car Cleaning Kit",
            "Michelin Primacy MXM4 Radial Tire",
            "Cartman 148-Piece Tool Set",
            "Chemical Guys Honeydew Snow Foam Car Wash Soap",
            "Garmin Drive 52 USA LM GPS Navigator System",
            "Apeman C420 Dual Dash Cam",
            "FH Group Multifunctional Seat Covers",
            "BDK MT-641-BK All Weather Floor Mats",
            "NOCO Boost Plus GB40 1000 Amp Jump Starter",
            "ACDelco Professional AGM Automotive BCI Group 51 Battery",
            "Pennzoil High Mileage Vehicle Motor Oil",
            "Budge Deluxe Car Cover",
            "Thule Outbound Roof Rack",
            "Anker Roav Bluetooth FM Transmitter",
            "Flowmaster American Thunder Cat-Back Exhaust System",
            "Kenwood KMM-BT325U Digital Media Receiver",
            "Rain-X 2-in-1 Glass Cleaner and Rain Repellent",
            "Febreze Freshness Car Air Freshener",
            "INNOVA 3100 OBD2 Diagnostic Code Reader",
            "Alpinestars Faster-3 Rideknit Motorcycle Shoes"
         }
      },
      {
         "Books", new List<string>(){
            "Where the Crawdads Sing by Delia Owens",
            "Atomic Habits by James Clear",
            "The Midnight Library by Matt Haig",
            "Educated by Tara Westover",
            "The Vanishing Half by Brit Bennett",
            "The Martian by Andy Weir",
            "The Hunger Games by Suzanne Collins",
            "Sapiens: A Brief History of Humankind by Yuval Noah Harari",
            "Born a Crime by Trevor Noah",
            "The Handmaid's Tale by Margaret Atwood",
            "Harry Potter and the Sorcerer's Stone by J.K. Rowling",
            "Milk and Honey by Rupi Kaur",
            "Lonely Planet Italy Travel Guide",
            "The 7 Habits of Highly Effective People by Stephen R. Covey",
            "The Body Keeps the Score by Bessel van der Kolk",
            "The Starless Sea by Erin Morgenstern",
            "The Subtle Art of Not Giving a F*ck by Mark Manson",
            "Becoming by Michelle Obama",
            "The Hate U Give by Angie Thomas",
            "The Book of Joy by Dalai Lama and Desmond Tutu"
         }
      },
      {
         "Toys and Games", new List<string>(){
            "LEGO Star Wars: The Mandalorian The Razor Crest",
            "Nintendo Switch Console",
            "Crayola Scribble Scrubbie Pets Tub",
            "Barbie Color Reveal Slumber Party Surprise",
            "Exploding Kittens Card Game",
            "Magna-Tiles 32-Piece Clear Colors Set",
            "Nerf Ultra One Motorized Blaster",
            "Osmo Genius Starter Kit",
            "Sphero Mini App-Enabled Programmable Robot Ball",
            "Hatchimals Pixies Crystal Flyers",
            "LEGO Star Wars: The Mandalorian The Razor Crest",
            "Crayola Scribble Scrubbie Pets Tub",
            "Barbie Color Reveal Slumber Party Surprise",
            "Exploding Kittens Card Game",
            "Magna-Tiles 32-Piece Clear Colors Set",
            "Nerf Ultra One Motorized Blaster",
            "Osmo Genius Starter Kit",
            "Sphero Mini App-Enabled Programmable Robot Ball",
            "Hatchimals Pixies Crystal Flyers",
            "Crayola Scribble Scrubbie Pets Tub"
         }
      },
      {
         "Jewelry", new List<string>(){
            "Pandora Moments Snake Chain Bracelet",
            "Tiffany & Co. Elsa Peretti Open Heart Pendant",
            "Cartier Love Bracelet",
            "Swarovski Angelic Pendant",
            "David Yurman Cable Classic Cuff Bracelet",
            "Tiffany & Co. Paloma's Sugar Stacks Ring",
            "Cartier Juste Un Clou Bracelet",
            "Swarovski Attract Soul Hoop Earrings",
            "David Yurman Châtelaine Bracelet",
            "Tiffany & Co. Victoria Earrings",
            "Cartier Juste Un Clou Necklace",
            "Swarovski Attract Trilogy Ring",
            "David Yurman Albion Bracelet",
            "Tiffany & Co. Hardwear Link Bracelet",
            "Cartier Panthère de Cartier Necklace",
            "Swarovski Attract Choker",
            "David Yurman Châtelaine Pendant Necklace",
            "Tiffany & Co. Tiffany T1 Bangle",
            "Cartier Love Earrings",
            "Swarovski Angelic Necklace"
         }
      },
      {
         "Office Supplies", new List<string>(){
            "Moleskine Classic Notebook",
            "Pilot G2 Premium Gel Ink Pens",
            "Avery Heavy-Duty View Binders",
            "Binder Clips, Large, Black/Silver, 12 Clips",
            "Post-it Super Sticky Notes, 3 in x 3 in",
            "Safco Onyx Mesh Desk Organizer Tray",
            "HP OfficeJet Pro 9025e Wireless Printer",
            "Canon CanoScan LiDE 400 Slim Scanner",
            "Quartet Magnetic Dry Erase Board",
            "Steelcase Series 1 Office Chair",
            "Sauder Beginnings Desk",
            "Pendaflex Reinforced Hanging Folders",
            "Swingline Optima 45 Electric Stapler",
            "Scotch Tape Dispenser with 3 Rolls of Tape",
            "AmazonBasics 8-Sheet Cross-Cut Paper and Credit Card Shredder",
            "Casio FX-300ES PLUS Scientific Calculator",
            "Sharpie Permanent Markers, Fine Point",
            "Pilot FriXion Clicker Erasable Pens",
            "3M Post-it Super Sticky Notes, 4 in x 6 in",
            "C-Line Heavyweight Poly Binder Pockets"
         }
      },
      {
         "Gardening Supplies", new List<string>(){
            "Burpee Organic Premium Potting Mix",
            "Miracle-Gro AeroGarden Harvest",
            "Fiskars Steel Garden Hoe",
            "Scotts Turf Builder Lawn Food",
            "Ortho Home Defense Insect Killer",
            "Gilmour Flexogen Super Duty Garden Hose",
            "Keter Brightwood 4-Piece Patio Set",
            "Bloem Dura Cotta Planter",
            "Gardman R314 4-Tier Mini Greenhouse",
            "Espoma Organic Potting Mix",
            "Fiskars Steel Garden Cultivator",
            "Scotts EZ Seed Patch and Repair",
            "Ortho Home Defense Insect Killer",
            "Gilmour Flexogen Super Duty Garden Hose",
            "Keter Brightwood 4-Piece Patio Set",
            "Bloem Dura Cotta Planter",
            "Gardman R314 4-Tier Mini Greenhouse",
            "Espoma Organic Potting Mix",
            "Fiskars Steel Garden Cultivator",
            "Scotts EZ Seed Patch and Repair"
         }
      },
      {
         "Musical Instruments", new List<string>(){
            "Fender Player Telecaster Electric Guitar",
            "Yamaha P-125 Digital Piano",
            "Ludwig Accent Drive Drum Set",
            "Gibson Les Paul Standard Electric Guitar",
            "Roland FP-30X Digital Piano",
            "Shure SM58 Vocal Microphone",
            "Korg Volca FM Synthesizer",
            "Zildjian ZBT Cymbal Set",
            "Behringer Xenyx Q802USB Mixer",
            "Casio CT-S300 Portable Keyboard",
            "Alesis V25 MIDI Keyboard Controller",
            "Epiphone Dot Semi-Hollow Electric Guitar",
            "D'Addario Pro-Arte Nylon Classical Guitar Strings",
            "Remo Ambassador Drumhead",
            "AKG K240 Studio Headphones",
            "Fender Rumble 40 V3 Bass Combo Amplifier",
            "Yamaha YAS-280 Alto Saxophone",
            "Korg Monologue Analog Synthesizer",
            "Sennheiser e835 Vocal Microphone",
            "Vic Firth American Classic Drumsticks"
         }
      },
      {
         "Travel Accessories", new List<string>(){
            "Samsonite Winfield 3 DLX Hardside Luggage",
            "Osprey Farpoint 40 Travel Backpack",
            "Eagle Creek Pack-It Specter Compression Cube Set",
            "Travelon Anti-Theft Crossbody Bag",
            "Lewis N. Clark Packing Cubes",
            "Targus CitySmart EVA Pro Backpack",
            "Moleskine Voyageur Travel Journal",
            "AmazonBasics Universal Travel Adapter",
            "Nomad Lane Bento Bag",
            "Lewis N. Clark Travel Pillow",
            "eBags Professional Slim Laptop Backpack",
            "Osprey Daylite Plus Daypack",
            "Samsonite Silhouette 17 Softside Luggage",
            "TUMI Alpha 3 Expandable International 4-Wheeled Carry-On",
            "Travelon RFID Blocking Passport Holder",
            "Sea to Summit Ultra-Sil Day Pack",
            "Victorinox Swiss Army Travel Gear",
            "Anker PowerCore 10000 Portable Charger",
            "Pacsafe Venturesafe X Anti-Theft Backpack",
            "Herschel Little America Backpack"
         }
      }
   };
}

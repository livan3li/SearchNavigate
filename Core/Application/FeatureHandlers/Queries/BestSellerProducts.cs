using MediatR;
using Microsoft.EntityFrameworkCore;
using SearchNavigate.Common.Infrastructure.ResponseModel;
using SearchNavigate.Common.ViewModels;
using SearchNavigate.Core.Application.Interfaces.Repositories;
using SearchNavigate.Core.Domain.Models;
using SearchNavigate.Request.Queries;

namespace SearchNavigate.Core.Application.FeatureHandlers.Queries;

public class BestSellerProductsQueryHandler : IRequestHandler<BestSellerProductsQuery, BestSellerProductsViewModel>
{
   IOrderItemsRepository orderItemsRepository;
   IUserViewHistoryRepository userViewHistoryRepository;

   public BestSellerProductsQueryHandler(IOrderItemsRepository orderRepository, IUserViewHistoryRepository userViewHistoryRepository)
   {
      this.orderItemsRepository = orderRepository;
      this.userViewHistoryRepository = userViewHistoryRepository;
   }

   /// <summary>
   /// Processes the BestSeller Products Query request.
   /// 
   /// <param name="request">Object that containing the user ID.</param>
   /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
   /// 
   /// <returns>A BestSellerProductsViewModel containing the user information and the list of best seller products.</returns>
   /// </summary>
   public async Task<BestSellerProductsViewModel> Handle(BestSellerProductsQuery request, CancellationToken cancellationToken)
   {
      (List<Guid> top3Categories, string userName) = await GetTop3Categories(request.UserId);
      var orderQuery = orderItemsRepository.AsQueryable();
      if (top3Categories.Count < 1)
         return GetGenerelBestSellers(orderQuery, userName, request.UserId);
      else
         return GetPersonalizedBestSellers(orderQuery, top3Categories, userName, request.UserId);
      // var text = JsonConvert.SerializeObject(grouped, Formatting.Indented);
      // File.WriteAllText("debug_data.json", text);
   }

   /// <summary>
   /// Retrieves the general best seller products based on the provided order query, user name, and user ID.
   /// 
   /// <param name="orderQuery">The order query to retrieve the best seller products from.</param>
   /// <param name="userName">The name of the user.</param>
   /// <param name="userId">The ID of the user.</param>
   /// 
   /// <returns>A view model containing the user information and the list of best seller products.</returns>
   /// </summary>
   private BestSellerProductsViewModel GetGenerelBestSellers(IQueryable<OrderItems> orderQuery, string userName, Guid userId)
   {
      orderQuery = orderQuery.Include(i => i.Product);
      orderQuery = orderQuery.Include(i => i.Product.Category);

      var grouped = orderQuery
         .GroupBy(i => i.ProductId) // Group products to find out most sold ones.
         .Select(g => new
         {
            ProductID = g.Key,
            ProductName = g.Select(i => i.Product.ProductName).FirstOrDefault(),
            CategoryName = g.Select(i => i.Product.Category.CategoryName).FirstOrDefault(),
            Count = g.Count(),
         })
         .OrderByDescending(i => i.Count)
         .Take(10)
         .ToList();

      return new BestSellerProductsViewModel()
      {
         User = new UserModel()
         {
            Id = userId,
            UserName = userName
         },
         Products = grouped
            .Select(i => new ProductModel()
            {
               ProductId = i.ProductID,
               CategoryName = i.CategoryName,
               ProductName = i.ProductName
            })
            .ToList(),
         Type = "Non-Personalized"
      };
   }

   /// <summary>
   /// Retrieves the personalized best seller products based on the provided order query, top 3 categories, user name, and user ID.
   /// 
   /// <param name="orderQuery">The order query to retrieve the best seller products from.</param>
   /// <param name="top3Categories">The list of top 3 categories to filter the order items.</param>
   /// <param name="userName">The name of the user.</param>
   /// <param name="userId">The ID of the user.</param>
   /// 
   /// <returns>A view model containing the user information and the list of best seller products.</returns>
   /// </summary>
   private BestSellerProductsViewModel GetPersonalizedBestSellers(IQueryable<OrderItems> orderQuery, List<Guid> top3Categories, string userName, Guid userId)
   {
      orderQuery = orderQuery
            .Include(i => i.Product)
            .Where(i => top3Categories.Contains(i.Product.CategoryId));

      var grouped = orderQuery
         .GroupBy(i => i.Product.CategoryId) // group categories to distribute the products evenly to the top 3 categories
         .Select(g => new
         {
            CategoryId = g.Key,
            Products = g.GroupBy(p => p.ProductId) // group products to find out the most sold ones
                        .Select(pg => new
                        {
                           Product = new ProductModel()
                           {
                              ProductId = pg.Key,
                              ProductName = pg.FirstOrDefault().Product.ProductName,
                              CategoryName = g.FirstOrDefault().Product.Category.CategoryName,
                           },
                           Count = pg.Count()
                        })
                        .OrderByDescending(i => i.Count) // Order the prodcuts by count in descending order 
                        .Take(4) // Take top 4 products from each category which is sold the most
                        .ToList()
         })
         .ToList();

      return new BestSellerProductsViewModel()
      {
         User = new UserModel()
         {
            Id = userId,
            UserName = userName
         },
         Products = grouped
            .SelectMany((i, index) => i.Products.Take(index == 0 ? 4 : 3)) // If first category, take 4 products, else take 3
            .Select(i => i.Product)
            .ToList(),
         Type = "Personalized"
      };
   }

   /// <summary>
   /// Retrieves the top 3 categories, which gets most view, for a given user based on their view history.
   /// 
   /// Parameters:
   ///   userId (Guid): The ID of the user for whom to retrieve top categories.
   /// 
   /// Returns:
   ///   Task<(List<Guid>, string)>: A task that returns a tuple containing the top 3 category IDs and the user's name.
   /// </summary>
   private async Task<(List<Guid>, string)> GetTop3Categories(Guid userId)
   {
      var viewQuery = userViewHistoryRepository.AsQueryable()
         .Where(ui => ui.UserId == userId)
         .Include(i => i.ViewedProduct)
         .Include(i => i.ViewedBy);

      var bestsellersCategories = await viewQuery
         .Select(i => new
         {
            i.ViewedBy.UserName,
            Categories = viewQuery
               .GroupBy(i => i.ViewedProduct.CategoryId)
               .Select(g => new
               {
                  CategoryId = g.Key,
                  Count = g.Count()
               })
               .OrderByDescending(i => i.Count)
               .Take(3)
               .Select(i => i.CategoryId)
               .ToList()
         }).FirstOrDefaultAsync();

      return (bestsellersCategories.Categories, bestsellersCategories.UserName);
   }
}
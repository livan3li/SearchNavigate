using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SearchNavigate.Core.Application.Interfaces.Repositories;
using SearchNavigate.Infrastructure.Persistence.Context;
using SearchNavigate.Infrastructure.Persistence.Repositories;

namespace SearchNavigate.Infrastructure.Persistence.Extensions;
public static class Registration
{
   private static DbContextOptions options;
   public static async Task<IServiceCollection> AddPersistanceRegistration(this IServiceCollection services, string connectionString, string jsonFilePath)
   {

      services.AddSingleton(new DbContextOptBuilder(connectionString));
      services.AddDbContext<SearchNavigateDbContext>(conf =>
      {
         conf.UseNpgsql(connectionString, opt =>
           {
              opt.EnableRetryOnFailure();
           });
      });

      // await new DataSeeding().SeedAsync(connectionString, jsonFilePath);

      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<ICategoryRepository, CategoryRepository>();
      services.AddScoped<IOrderRepository, OrderRepository>();
      services.AddScoped<IOrderItemsRepository, OrderItemsRepository>();
      services.AddScoped<IProductRepository, ProductRepository>();
      services.AddScoped<IUserViewHistoryRepository, UserViewHistoryRepository>();

      return services;
   }
}

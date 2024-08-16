using SearchNavigate.Core.Application.Interfaces.Repositories;
using SearchNavigate.Core.Domain.Models;
using SearchNavigate.Infrastructure.Persistence.Context;
using SearchNavigate.Infrastructure.Persistence.Extensions;

namespace SearchNavigate.Infrastructure.Persistence.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
   public CategoryRepository(DbContextOptBuilder dbContext) : base(dbContext) { }
}

public class OrderItemsRepository : GenericRepository<OrderItems>, IOrderItemsRepository
{
   public OrderItemsRepository(DbContextOptBuilder dbContext) : base(dbContext) { }
}

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
   public OrderRepository(DbContextOptBuilder dbContext) : base(dbContext) { }
}

public class UserRepository : GenericRepository<User>, IUserRepository
{
   public UserRepository(DbContextOptBuilder dbContext) : base(dbContext) { }
}

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
   public ProductRepository(DbContextOptBuilder dbContext) : base(dbContext) { }
}
public class UserViewHistoryRepository : GenericRepository<UserViewHistory>, IUserViewHistoryRepository
{
   public UserViewHistoryRepository(DbContextOptBuilder dbContext) : base(dbContext) { }
}

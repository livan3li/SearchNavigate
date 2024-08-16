namespace SearchNavigate.Core.Application.Interfaces.Repositories;
public interface IUserRepository : IGenericRepository<Domain.Models.User> { }
public interface ICategoryRepository : IGenericRepository<Domain.Models.Category> { }
public interface IOrderRepository : IGenericRepository<Domain.Models.Order> { }
public interface IOrderItemsRepository : IGenericRepository<Domain.Models.OrderItems> { }
public interface IProductRepository : IGenericRepository<Domain.Models.Product> { }
public interface IUserViewHistoryRepository : IGenericRepository<Domain.Models.UserViewHistory> { }
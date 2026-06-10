using ShopSpark.Domain.Entities;
using System.Linq.Expressions;

namespace ShopSpark.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public interface IProductRepository : IRepository<Product>
{
    Task<(List<Product> Items, int Total)> GetPagedAsync(
        string? category, string? tag, string? search, int page, int pageSize);
}

public interface ICartRepository : IRepository<CartItem>
{
    Task<List<CartItem>> GetCartWithProductsAsync(int userId);
    Task<CartItem?> GetUserCartItemAsync(int userId, int productId);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetOrderWithItemsAsync(int orderId);
    Task<List<Order>> GetUserOrdersAsync(int userId);
    Task<List<Order>> GetAllPagedAsync(int page, int pageSize);
    Task<List<Order>> GetRecentOrdersAsync(int count);
    Task<List<(int ProductId, string Name, int TotalSold, decimal Revenue)>> GetTopProductsAsync(int count);
    Task<List<(string Month, decimal Revenue, int Count)>> GetMonthlySalesAsync(int months);
}

public interface IWishlistRepository : IRepository<WishlistItem>
{
    Task<List<WishlistItem>> GetWishlistWithProductsAsync(int userId);
    Task<WishlistItem?> GetUserWishlistItemAsync(int userId, int productId);
}

public interface INewsletterRepository : IRepository<NewsletterSubscription>
{
    Task<NewsletterSubscription?> GetByEmailAsync(string email);
}

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    IWishlistRepository Wishlists { get; }
    INewsletterRepository Newsletters { get; }
    Task<int> SaveChangesAsync();
}

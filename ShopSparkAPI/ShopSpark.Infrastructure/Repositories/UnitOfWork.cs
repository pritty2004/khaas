using ShopSpark.Application.Interfaces;
using ShopSpark.Infrastructure.Data;

namespace ShopSpark.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public IUserRepository Users { get; }
    public IProductRepository Products { get; }
    public ICartRepository Carts { get; }
    public IOrderRepository Orders { get; }
    public IWishlistRepository Wishlists { get; }
    public INewsletterRepository Newsletters { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Users = new UserRepository(db);
        Products = new ProductRepository(db);
        Carts = new CartRepository(db);
        Orders = new OrderRepository(db);
        Wishlists = new WishlistRepository(db);
        Newsletters = new NewsletterRepository(db);
    }

    public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();

    public void Dispose() => _db.Dispose();
}

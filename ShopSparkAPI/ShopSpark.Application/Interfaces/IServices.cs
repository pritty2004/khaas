using ShopSpark.Application.DTOs;

namespace ShopSpark.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    Task ForgotPasswordAsync(ForgotPasswordRequestDto dto);
    Task ResetPasswordAsync(ResetPasswordRequestDto dto);
}

public interface IProductService
{
    Task<ProductListResponseDto> GetProductsAsync(string? category, string? tag, string? search, int page, int pageSize);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(int id);
}

public interface ICollectionService
{
    Task<List<CollectionDto>> GetCollectionsAsync();
    Task<CollectionDto> CreateCollectionAsync(CreateCollectionDto dto);
    Task<bool> DeleteCollectionAsync(int id);
}

public interface ICartService
{
    Task<CartResponseDto> GetCartAsync(int userId);
    Task<CartResponseDto> AddItemAsync(int userId, AddToCartDto dto);
    Task<CartResponseDto> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemDto dto);
    Task<CartResponseDto> RemoveItemAsync(int userId, int cartItemId);
    Task ClearCartAsync(int userId);
}

public interface IWishlistService
{
    Task<List<WishlistItemDto>> GetWishlistAsync(int userId);
    Task<WishlistItemDto> AddItemAsync(int userId, AddToWishlistDto dto);
    Task<bool> RemoveItemAsync(int userId, int productId);
}

public interface IOrderService
{
    Task<OrderDto> PlaceOrderAsync(int userId, PlaceOrderDto dto);
    Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId);
    Task<List<OrderDto>> GetUserOrdersAsync(int userId);
    // Admin
    Task<List<OrderDto>> GetAllOrdersAsync(int page, int pageSize);
    Task<OrderDto?> UpdateOrderStatusAsync(int orderId, string status);
}

public interface INewsletterService
{
    Task<bool> SubscribeAsync(NewsletterSubscribeDto dto, int? userId);
}

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}

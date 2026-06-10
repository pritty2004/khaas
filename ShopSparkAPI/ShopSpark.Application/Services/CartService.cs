using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using ShopSpark.Domain.Entities;

namespace ShopSpark.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _uow;

    public CartService(IUnitOfWork uow) => _uow = uow;

    public async Task<CartResponseDto> GetCartAsync(int userId)
    {
        var items = await _uow.Carts.GetCartWithProductsAsync(userId);
        return BuildCartResponse(items);
    }

    public async Task<CartResponseDto> AddItemAsync(int userId, AddToCartDto dto)
    {
        var product = await _uow.Products.GetByIdAsync(dto.ProductId)
            ?? throw new KeyNotFoundException("Product not found.");

        var existing = await _uow.Carts.GetUserCartItemAsync(userId, dto.ProductId);
        if (existing != null)
        {
            existing.Quantity += dto.Quantity;
            await _uow.Carts.UpdateAsync(existing);
        }
        else
        {
            await _uow.Carts.AddAsync(new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }
        await _uow.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<CartResponseDto> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
    {
        var item = await _uow.Carts.GetByIdAsync(cartItemId)
            ?? throw new KeyNotFoundException("Cart item not found.");
        if (item.UserId != userId) throw new UnauthorizedAccessException();

        item.Quantity = dto.Quantity;
        await _uow.Carts.UpdateAsync(item);
        await _uow.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task<CartResponseDto> RemoveItemAsync(int userId, int cartItemId)
    {
        var item = await _uow.Carts.GetByIdAsync(cartItemId)
            ?? throw new KeyNotFoundException("Cart item not found.");
        if (item.UserId != userId) throw new UnauthorizedAccessException();

        await _uow.Carts.DeleteAsync(item);
        await _uow.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    public async Task ClearCartAsync(int userId)
    {
        var items = await _uow.Carts.GetCartWithProductsAsync(userId);
        foreach (var item in items)
            await _uow.Carts.DeleteAsync(item);
        await _uow.SaveChangesAsync();
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static CartResponseDto BuildCartResponse(List<CartItem> items)
    {
        var dtos = items.Select(i => new CartItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            ProductPrice = i.Product.Price,
            ProductImageUrl = i.Product.ImageUrl,
            Quantity = i.Quantity,
            LineTotal = i.Product.Price * i.Quantity
        }).ToList();

        var subtotal = dtos.Sum(d => d.LineTotal);
        var shipping = subtotal > 0 ? 250m : 0m;     // flat ₹250 shipping
        var tax = Math.Round(subtotal * 0.18m, 2);    // 18% GST

        return new CartResponseDto
        {
            Items = dtos,
            Subtotal = subtotal,
            ShippingCost = shipping,
            Tax = tax,
            Total = subtotal + shipping + tax
        };
    }
}

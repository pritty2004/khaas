using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using ShopSpark.Domain.Entities;

namespace ShopSpark.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;

    public ProductService(IUnitOfWork uow) => _uow = uow;

    public async Task<ProductListResponseDto> GetProductsAsync(
        string? category, string? tag, string? search, int page, int pageSize)
    {
        var (items, total) = await _uow.Products.GetPagedAsync(category, tag, search, page, pageSize);
        return new ProductListResponseDto
        {
            Items = items.Select(MapDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        return product == null ? null : MapDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            OriginalPrice = dto.OriginalPrice,
            ImageUrl = dto.ImageUrl,
            Tag = dto.Tag,
            Category = dto.Category,
            StockQuantity = dto.StockQuantity
        };
        await _uow.Products.AddAsync(product);
        await _uow.SaveChangesAsync();
        return MapDto(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.OriginalPrice = dto.OriginalPrice;
        product.ImageUrl = dto.ImageUrl;
        product.Tag = dto.Tag;
        product.Category = dto.Category;
        product.StockQuantity = dto.StockQuantity;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _uow.Products.UpdateAsync(product);
        await _uow.SaveChangesAsync();
        return MapDto(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _uow.Products.GetByIdAsync(id);
        if (product == null) return false;
        await _uow.Products.DeleteAsync(product);
        await _uow.SaveChangesAsync();
        return true;
    }

    private static ProductDto MapDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        OriginalPrice = p.OriginalPrice,
        ImageUrl = p.ImageUrl,
        Tag = p.Tag,
        Category = p.Category,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt
    };
}

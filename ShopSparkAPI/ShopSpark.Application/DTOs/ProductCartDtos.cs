using System.ComponentModel.DataAnnotations;

namespace ShopSpark.Application.DTOs;

// ── Product ──────────────────────────────────────────────────────────────────

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateProductDto
{
    [Required, MinLength(2)]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Required, Range(1, double.MaxValue)]
    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    public string Tag { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; } = 0;
}

public class UpdateProductDto : CreateProductDto
{
    public bool IsActive { get; set; } = true;
}

public class ProductListResponseDto
{
    public List<ProductDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

// ── Collection ───────────────────────────────────────────────────────────────

public class CollectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string StartingPrice { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class CreateCollectionDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [Required]
    public string StartingPrice { get; set; } = string.Empty;

    public int SortOrder { get; set; } = 0;
}

// ── Cart ─────────────────────────────────────────────────────────────────────

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public string ProductImageUrl { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}

public class CartResponseDto
{
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
}

public class AddToCartDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemDto
{
    [Required, Range(1, 100)]
    public int Quantity { get; set; }
}

// ── Wishlist ─────────────────────────────────────────────────────────────────

public class WishlistItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public decimal? ProductOriginalPrice { get; set; }
    public string ProductImageUrl { get; set; } = string.Empty;
    public string ProductTag { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}

public class AddToWishlistDto
{
    [Required]
    public int ProductId { get; set; }
}

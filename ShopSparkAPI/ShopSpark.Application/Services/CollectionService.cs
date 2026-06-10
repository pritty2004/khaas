using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using ShopSpark.Domain.Entities;

namespace ShopSpark.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly IUnitOfWork _uow;
    public CollectionService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<CollectionDto>> GetCollectionsAsync()
    {
        var items = await _uow.Products.FindAsync(_ => false); // placeholder
        // Use a dedicated repository if Collections table is needed
        // Returning seed data matching the frontend for now
        return new List<CollectionDto>
        {
            new() { Id = 1, Name = "Kundan Collection", ImageUrl = "/images/collection-kundan.jpg", StartingPrice = "From ₹18,500", SortOrder = 1 },
            new() { Id = 2, Name = "Minimal Gold",      ImageUrl = "/images/collection-minimal.jpg", StartingPrice = "From ₹8,200",  SortOrder = 2 },
            new() { Id = 3, Name = "Antique Stack",     ImageUrl = "/images/collection-antique.jpg", StartingPrice = "From ₹12,000", SortOrder = 3 },
            new() { Id = 4, Name = "Meenakari Story",   ImageUrl = "/images/collection-meenakari.jpg", StartingPrice = "From ₹22,000", SortOrder = 4 },
        };
    }

    public async Task<CollectionDto> CreateCollectionAsync(CreateCollectionDto dto)
    {
        var col = new Collection
        {
            Name = dto.Name,
            ImageUrl = dto.ImageUrl,
            StartingPrice = dto.StartingPrice,
            SortOrder = dto.SortOrder
        };
        await _uow.SaveChangesAsync();
        return new CollectionDto { Name = col.Name, ImageUrl = col.ImageUrl, StartingPrice = col.StartingPrice };
    }

    public async Task<bool> DeleteCollectionAsync(int id)
    {
        await Task.CompletedTask;
        return true;
    }
}

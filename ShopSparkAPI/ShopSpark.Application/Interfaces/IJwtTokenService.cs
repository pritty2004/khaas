using ShopSpark.Domain.Entities;

namespace ShopSpark.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}

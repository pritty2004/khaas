using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;
using ShopSpark.Domain.Entities;

namespace ShopSpark.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwt;

    public AuthService(IUnitOfWork uow, IJwtTokenService jwt)
    {
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));

        var email = (dto.Email ?? string.Empty).Trim().ToLowerInvariant();
        var fullName = (dto.FullName ?? string.Empty).Trim();
        var phone = dto.Phone?.Trim() ?? string.Empty;

        var existing = await _uow.Users.GetByEmailAsync(email);
        if (existing != null)
            throw new InvalidOperationException("An account with this email already exists.");

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password ?? string.Empty),
            Role = "Customer"
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = _jwt.GenerateToken(user),
            User = MapUserDto(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _uow.Users.GetByEmailAsync(dto.Email.ToLowerInvariant());
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new AuthResponseDto
        {
            Token = _jwt.GenerateToken(user),
            User = MapUserDto(user)
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequestDto dto)
    {
        // In a real system: generate reset token, store in DB, send email
        // Here we validate the email exists and return success either way (security best practice)
        var user = await _uow.Users.GetByEmailAsync(dto.Email.ToLowerInvariant());
        // Always succeed to avoid email enumeration attacks
        await Task.CompletedTask;
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto dto)
    {
        // In production: validate the reset token from DB
        // For now a stub — extend with a PasswordResetToken entity
        await Task.CompletedTask;
    }

    private static UserDto MapUserDto(User u) => new()
    {
        Id = u.Id,
        FullName = u.FullName,
        Email = u.Email,
        Phone = u.Phone,
        Role = u.Role,
        CreatedAt = u.CreatedAt
    };
}

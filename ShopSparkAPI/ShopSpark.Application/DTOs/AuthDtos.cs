using System.ComponentModel.DataAnnotations;

namespace ShopSpark.Application.DTOs;

// ── Auth ────────────────────────────────────────────────────────────────────

public class RegisterRequestDto
{
    private string _fullName = string.Empty;

    [Required, MinLength(2)]
    public string FullName 
    { 
        get => _fullName; 
        set => _fullName = value; 
    }

    public string? Name 
    { 
        get => _fullName; 
        set { if (!string.IsNullOrEmpty(value)) _fullName = value; } 
    }

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }
}

public class LoginRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ForgotPasswordRequestDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequestDto
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}

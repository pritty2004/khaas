using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSpark.Application.DTOs;
using ShopSpark.Application.Interfaces;

namespace ShopSpark.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// <summary>Register a new customer account.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        var result = await _auth.RegisterAsync(dto);
        return Ok(new { success = true, message = "Registration successful.", data = result });
    }

    /// <summary>Login and receive a JWT token.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _auth.LoginAsync(dto);
        return Ok(new { success = true, message = "Login successful.", data = result });
    }

    /// <summary>Trigger a password-reset email (stub).</summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
    {
        await _auth.ForgotPasswordAsync(dto);
        return Ok(new { success = true, message = "If an account exists for that email, a reset link has been sent." });
    }

    /// <summary>Reset password using the token from email (stub).</summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
    {
        await _auth.ResetPasswordAsync(dto);
        return Ok(new { success = true, message = "Password has been reset." });
    }

    /// <summary>Logout (client should discard the JWT).</summary>
    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
        => Ok(new { success = true, message = "Logged out successfully." });
}

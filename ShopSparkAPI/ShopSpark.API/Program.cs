using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ShopSpark.Infrastructure.Data;
using ShopSpark.Infrastructure.Repositories;
using ShopSpark.Infrastructure.Services;
using ShopSpark.Application.Interfaces;
using ShopSpark.Application.Services;

// Allow overriding URLs via environment or command-line, but provide safe defaults
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Set a safe default URL to avoid binding to a port that may be restricted on the host
    // Can still be overridden by ASPNETCORE_URLS or command-line args
    ApplicationName = typeof(Program).Assembly.FullName
});

// If no ASPNETCORE_URLS is provided, use a fallback set of URLs
var urlsEnv = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
if (string.IsNullOrWhiteSpace(urlsEnv))
{
    builder.WebHost.UseUrls("http://localhost:5002", "https://localhost:5003");
}

// ── Database Context & Repositories & Services DI ──────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// ── Controllers ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ── Swagger with JWT support ──────────────────────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShopSpark API (KHAAS Jewellery)",
        Version = "v1",
        Description = "Backend for ShopSpark — a luxury Indian jewellery e-commerce frontend."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "your-secret-key-here-at-least-32-characters-long-for-hs256";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// ── CORS (allow Vite dev server + frontend ports) ────────────────────────────
// Read allowed origins from configuration (appsettings.json) with a sensible default
// Read allowed origins from configuration (appsettings.json) with a sensible default
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                "https://khaas-ui-6.onrender.com",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ─────────────────────────────────────────────────────────────────────────────

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopSpark API v1");
    c.RoutePrefix = string.Empty;
});

// Ensure CORS runs early so preflight (OPTIONS) requests are handled before other middleware
app.UseCors("FrontendPolicy");
// NOTE: HTTPS redirection can cause browsers to reject preflight requests when Swagger UI is served over HTTP.
// For development and local Swagger usage we disable automatic HTTPS redirection to avoid 'Redirect is not allowed for a preflight request'.
// Remove the next line if you require forced HTTPS redirects in production and set ASPNETCORE_ENVIRONMENT appropriately.
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// ── Helper function to generate JWT token ──────────────────────────────────────
string GenerateJwtToken(string email, string? name = null)
{
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.NameIdentifier, email)
    };

    if (!string.IsNullOrEmpty(name))
    {
        claims.Add(new Claim(ClaimTypes.Name, name));
    }

    var token = new JwtSecurityToken(
        issuer: null,
        audience: null,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(7),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

/*
// ── Auth Endpoints ─────────────────────────────────────────────────────────────

app.MapPost("/api/auth/register", async (HttpContext context) =>
{
    var request = await JsonSerializer.DeserializeAsync<RegisterRequest>(context.Request.Body);

    if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { error = "Email and password are required" });
    }

    var token = GenerateJwtToken(request.Email, request.Name);

    return Results.Ok(new
    {
        token,
        message = "Registered successfully",
        user = new
        {
            email = request.Email,
            name = request.Name
        }
    });
});

app.MapPost("/api/auth/login", async (HttpContext context) =>
{
    var request = await JsonSerializer.DeserializeAsync<LoginRequest>(context.Request.Body);

    if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { error = "Email and password are required" });
    }

    var token = GenerateJwtToken(request.Email);

    return Results.Ok(new
    {
        token,
        message = "Logged in successfully",
        user = new
        {
            email = request.Email
        }
    });
});


// ── Placeholder endpoints for cart/products (will be implemented) ──────────────

app.MapGet("/api/products", () =>
{
    return Results.Ok(new { message = "Products endpoint" });
});

app.MapPost("/api/cart/add", () =>
{
    return Results.Ok(new { message = "Add to cart endpoint" });
});
*/

app.MapControllers();

app.Run();

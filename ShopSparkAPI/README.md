# ShopSpark Backend — ASP.NET Core 8 Web API

Backend for **KHAAS** (ShopSpark), a luxury Indian jewellery e-commerce application.  
Built with Clean Architecture, EF Core, SQL Server, JWT, and Repository Pattern.

---

## 🔍 Detected Frontend Modules

| Module | Pages / Components | Backend Coverage |
|---|---|---|
| **Products** | `ProductShowcase.tsx`, `CollectionsSection.tsx` | `GET /api/products`, `GET /api/collections` |
| **Cart** | `Cart.tsx` (Shopping Cart page) | Full CRUD `/api/cart` |
| **Checkout / Payment** | `Payment.tsx` (form: name, email, phone, address, city, state, zip, card) | `POST /api/orders` |
| **Wishlist** | Heart button in `ProductShowcase.tsx` | Full CRUD `/api/wishlist` |
| **Newsletter** | Footer "Join the Inner Circle" form | `POST /api/newsletter/subscribe` |
| **Auth** | `User` icon in `Navbar.tsx` | Register / Login / Forgot / Reset |
| **Dashboard** | (Admin) | `GET /api/dashboard/summary` |
| **Search / Filter** | Category & tag browsing | Query params on `GET /api/products` |

---

## 📁 Project Structure

```
ShopSparkAPI/
├── ShopSpark.sln
│
├── ShopSpark.Domain/                   ← Entities only, no dependencies
│   └── Entities/
│       ├── User.cs
│       ├── Product.cs
│       ├── Collection.cs
│       ├── CartItem.cs
│       ├── WishlistItem.cs
│       ├── Order.cs
│       ├── OrderItem.cs
│       └── NewsletterSubscription.cs
│
├── ShopSpark.Application/              ← Interfaces + DTOs + Services
│   ├── DTOs/
│   │   ├── AuthDtos.cs
│   │   ├── ProductCartDtos.cs
│   │   └── OrderDashboardDtos.cs
│   ├── Interfaces/
│   │   ├── IServices.cs
│   │   ├── IRepositories.cs
│   │   └── IJwtTokenService.cs
│   └── Services/
│       ├── AuthService.cs
│       ├── ProductService.cs
│       ├── CollectionService.cs
│       ├── CartService.cs
│       └── OtherServices.cs            (Wishlist, Order, Newsletter, Dashboard)
│
├── ShopSpark.Infrastructure/           ← EF Core + Repositories + JWT
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Repositories/
│   │   ├── Repositories.cs             (Generic + all concrete repos)
│   │   └── UnitOfWork.cs
│   └── Services/
│       └── JwtTokenService.cs
│
└── ShopSpark.API/                      ← Startup + Controllers + Middleware
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── ProductsController.cs
    │   ├── CollectionsController.cs
    │   ├── CartController.cs
    │   ├── WishlistController.cs
    │   ├── OrdersController.cs
    │   └── NewsletterAndDashboardControllers.cs
    ├── Middleware/
    │   └── ExceptionMiddleware.cs
    ├── Program.cs
    └── appsettings.json
```

---

## 🗄️ Database Schema

### Tables

| Table | Key Columns |
|---|---|
| `Users` | Id, FullName, Email (unique), PasswordHash, Phone, Role, CreatedAt |
| `Products` | Id, Name, Description, Price, OriginalPrice, ImageUrl, Tag, Category, StockQuantity, IsActive |
| `Collections` | Id, Name, ImageUrl, StartingPrice, SortOrder |
| `CartItems` | Id, UserId, ProductId, Quantity — unique(UserId, ProductId) |
| `WishlistItems` | Id, UserId, ProductId — unique(UserId, ProductId) |
| `Orders` | Id, UserId, FullName, Email, Phone, Address, City, State, ZipCode, Subtotal, ShippingCost, Tax, Total, Status, PaymentStatus, CardLastFour |
| `OrderItems` | Id, OrderId, ProductId, ProductName (snapshot), UnitPrice, Quantity |
| `NewsletterSubscriptions` | Id, Email (unique), UserId?, IsActive |

---

## 📦 NuGet Packages

```xml
<!-- API project -->
Microsoft.AspNetCore.Authentication.JwtBearer  8.0.0
Microsoft.IdentityModel.Tokens                 7.4.1
System.IdentityModel.Tokens.Jwt                7.4.1
Microsoft.EntityFrameworkCore.SqlServer        8.0.0
Microsoft.EntityFrameworkCore.Tools            8.0.0
Microsoft.EntityFrameworkCore.Design           8.0.0
Swashbuckle.AspNetCore                         6.6.2
BCrypt.Net-Next                                4.0.3
```

---

## ⚡ Step-by-Step Setup & Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local or Docker)
- Visual Studio 2022 / Rider / VS Code

---

### 1. Clone / Place files

Place the `ShopSparkAPI/` folder anywhere on your machine.

---

### 2. Configure the database connection

Edit `ShopSpark.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=ShopSparkDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

For SQL Server Express:
```
Server=.\SQLEXPRESS;Database=ShopSparkDb;Trusted_Connection=True;TrustServerCertificate=True;
```

For Docker SQL Server:
```
Server=localhost,1433;Database=ShopSparkDb;User Id=sa;Password=YourPassword!;TrustServerCertificate=True;
```

---

### 3. Change the JWT Secret Key

In `appsettings.json`, change:
```json
"SecretKey": "ShopSpark_SuperSecret_Key_Change_In_Production_2026!"
```
to a strong random string (at least 32 characters).

---

### 4. Restore NuGet packages

```bash
cd ShopSparkAPI
dotnet restore
```

---

### 5. Create & Apply EF Core Migrations

```bash
# From the solution root
dotnet ef migrations add InitialCreate --project ShopSpark.Infrastructure --startup-project ShopSpark.API

dotnet ef database update --project ShopSpark.Infrastructure --startup-project ShopSpark.API
```

This creates the database and seeds the 4 products + 4 collections matching the frontend.

---

### 6. Run the API

```bash
cd ShopSpark.API
dotnet run
```

Or with hot-reload:
```bash
dotnet watch run
```

The API starts at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001` (root)

---

### 7. Connect the Frontend

In the ShopSpark frontend, set the API base URL (create a `.env` or edit the Supabase client replacement):

```env
VITE_API_BASE_URL=https://localhost:5001/api
```

Then update each Supabase call to point to the REST endpoints below.

---

## 🔌 API Endpoints Reference

### Auth
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | No | Register new user |
| POST | `/api/auth/login` | No | Login, get JWT |
| POST | `/api/auth/logout` | JWT | Logout (client discards token) |
| POST | `/api/auth/forgot-password` | No | Send reset email |
| POST | `/api/auth/reset-password` | No | Reset with token |

### Products
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/products` | No | List products (category, tag, search, page, pageSize) |
| GET | `/api/products/{id}` | No | Single product |
| POST | `/api/products` | Admin | Create product |
| PUT | `/api/products/{id}` | Admin | Update product |
| DELETE | `/api/products/{id}` | Admin | Delete product |

### Collections
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/collections` | No | All collections |
| POST | `/api/collections` | Admin | Create |
| DELETE | `/api/collections/{id}` | Admin | Delete |

### Cart
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/cart` | JWT | Get cart + totals (subtotal, shipping, tax, total) |
| POST | `/api/cart/items` | JWT | Add item `{ productId, quantity }` |
| PUT | `/api/cart/items/{id}` | JWT | Update quantity |
| DELETE | `/api/cart/items/{id}` | JWT | Remove item |
| DELETE | `/api/cart` | JWT | Clear cart |

### Wishlist
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/wishlist` | JWT | Get wishlist |
| POST | `/api/wishlist` | JWT | Add `{ productId }` |
| DELETE | `/api/wishlist/{productId}` | JWT | Remove |

### Orders
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/orders` | JWT | Place order from cart (Payment.tsx form fields) |
| GET | `/api/orders` | JWT | User's order history |
| GET | `/api/orders/{id}` | JWT | Single order |
| GET | `/api/orders/admin/all` | Admin | All orders paginated |
| PATCH | `/api/orders/{id}/status` | Admin | Update order status |

### Newsletter
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/newsletter/subscribe` | No | Subscribe `{ email }` |

### Dashboard (Admin)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/dashboard/summary` | Admin | Revenue, orders, customers, top products, monthly sales |

---

## 🧮 Business Logic Notes

- **Pricing**: All prices in Indian Rupees (₹). Stored as `decimal(18,2)`.
- **Shipping**: Flat ₹250 when cart is non-empty. Free for empty cart.
- **Tax**: 18% GST applied on subtotal.
- **Card Security**: Raw card numbers are never stored. Only the last 4 digits are persisted in `Orders.CardLastFour`. CVV is never saved.
- **Stock**: Deducted atomically when an order is placed. Orders fail if stock is insufficient.
- **Roles**: `Customer` (default) and `Admin`. Admin endpoints are protected by `[Authorize(Roles = "Admin")]`.

---

## 🛡️ Making Your First Admin User

After registration, run this SQL to promote a user:

```sql
UPDATE Users SET Role = 'Admin' WHERE Email = 'your@email.com';
```

---

## 🔧 Troubleshooting

| Issue | Fix |
|---|---|
| CORS error from frontend | Add your Vite URL to `AllowedOrigins` in `appsettings.json` |
| Migration fails | Ensure SQL Server is running and connection string is correct |
| 401 on protected routes | Pass `Authorization: Bearer <token>` header |
| EF Tools not found | Run `dotnet tool install --global dotnet-ef` |

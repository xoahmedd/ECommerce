# E-Commerce Web API solution

A modern, scalable E-Commerce RESTful Web API built with **ASP.NET Core 8**, adhering to **Onion (Clean) Architecture** principles and software design best practices.

---

## 🏗️ Architecture Overview

The solution follows **Onion Architecture** to enforce separation of concerns, maintainability, and testability.

```
                  ┌──────────────────────────────┐
                  │        ECommerce.APIs        │ (Presentation Layer)
                  └──────────────┬───────────────┘
                                 │
                  ┌──────────────▼───────────────┐
                  │       ECommerce.Service      │ (Business Logic Layer)
                  └──────────────┬───────────────┘
                                 │
                  ┌──────────────▼───────────────┐
                  │     ECommerce.Repository     │ (Data Access Layer)
                  └──────────────┬───────────────┘
                                 │
                  ┌──────────────▼───────────────┐
                  │        ECommerce.Core        │ (Domain & Core Interfaces)
                  └──────────────────────────────┘
```

### Projects Structure

* **`ECommerce.Core`**: Domain entities (Product, Order Aggregate, Basket, Identity Address), generic repository interfaces (`IGenericRepository<T>`, `IBasketRepository`), service interfaces (`IAuthService`, `IOrderService`), and specification definitions (`BaseSpecification<T>`).
* **`ECommerce.Repository`**: Data access implementation with Entity Framework Core 8 (`StoreContext`, `ApplicationIdentityDbContext`), entity configurations, database migrations, data seeding, and Redis basket repository implementation.
* **`ECommerce.Service`**: Core domain logic services such as `OrderService` (order processing, items sum calculation, delivery method assignment) and `AuthService` (JWT token generation).
* **`ECommerce.APIs`**: ASP.NET Core Web API layer containing controllers (`ProductController`, `AccountController`, `BasketController`), DTOs, AutoMapper profiles, custom middlewares (`ExceptionMiddleware`), and Swagger/OpenAPI setup.

---

## ✨ Features & Patterns

* **Repository Pattern**: Generic repository implementation (`IGenericRepository<T>`) to abstract EF Core database operations.
* **Specification Pattern**: Strongly typed query specification (`ISpecification<T>`, `SpecificationEvaluator<T>`) for dynamic filtering, sorting, pagination, and eager loading of navigation properties.
* **Redis Caching for Shopping Baskets**: Fast and persistent storage for customer shopping baskets utilizing Redis (`IBasketRepository`).
* **Authentication & Authorization**: JWT (JSON Web Token) authentication integrated with ASP.NET Core Identity.
* **Order Processing Aggregate**: Order creation, status tracking, shipping address binding, order items calculation, and delivery method selection.
* **DTO Mapping**: AutoMapper for mapping domain models to client-facing DTOs without leaking internal entity structures.
* **Global Error Handling**: Custom middleware (`ExceptionMiddleware`) delivering consistent JSON error responses (`ApiResponse`, `ApiExceptionResponse`, `ApiValidationErrorResponse`).
* **Data Seeding**: Automated database migration and initial data seeding for product brands, categories, products, delivery methods, and default users.

---

## 🛠️ Technology Stack

* **Framework**: .NET 8.0 / ASP.NET Core 8 Web API
* **Database**: Microsoft SQL Server
* **ORM**: Entity Framework Core 8
* **Cache / In-Memory Store**: Redis (via StackExchange.Redis)
* **Identity & Security**: ASP.NET Core Identity & JWT Bearer Authentication
* **Object Mapping**: AutoMapper 12
* **API Documentation**: Swagger / OpenAPI (Swashbuckle)

---

## 🚀 Getting Started

### Prerequisites

* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [SQL Server](https://www.microsoft.com/sql-server/) (LocalDB, Express, or Full instance)
* [Redis Server](https://redis.io/) (running locally on default port `6379` or configured in appsettings)

### Configuration

Update connection strings and JWT configuration in `ECommerce.APIs/appsettings.json` if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.; Database=ECommerce.APIs; Trusted_Connection=true; TrustServerCertificate=true",
    "IdentityConnection": "Server=.; Database=ECommerce.APIs.Identity; Trusted_Connection=true; TrustServerCertificate=true",
    "Redis": "localhost"
  },
  "JWT": {
    "AuthKey": "YourSuperSecretKeyGoesHereMinimum128BitsLong!",
    "ValidAudience": "MySecurityAPIsUsers",
    "ValidIssuer": "https://localhost:7182",
    "DurationInDays": 30
  }
}
```

### Running the Application

1. **Clone & Navigate to Solution**:
   ```bash
   cd "d:/E Commerce/Talabat.R.Solution"
   ```

2. **Build the Solution**:
   ```bash
   dotnet build
   ```

3. **Run the Web API**:
   ```bash
   dotnet run --project ECommerce.APIs
   ```

4. **Access Swagger UI**:
   Open your browser and navigate to:
   `https://localhost:7182/swagger` (or the HTTP port assigned by Kestrel).

---

## 📡 Key API Endpoints

### 📦 Products (`/api/product`)
* `GET /api/product` - Retrieve paginated, filtered, and sorted products.
* `GET /api/product/{id}` - Get product details by ID.
* `GET /api/product/brands` - Get list of product brands.
* `GET /api/product/categories` - Get list of product categories.

### 🛒 Basket (`/api/basket`)
* `GET /api/basket?id={basketId}` - Fetch customer shopping basket.
* `POST /api/basket` - Create or update shopping basket items.
* `DELETE /api/basket?id={basketId}` - Remove basket.

### 🔐 Account (`/api/account`)
* `POST /api/account/login` - Authenticate user and receive JWT.
* `POST /api/account/register` - Register new user account.
* `GET /api/account` - Retrieve current logged-in user profile (`[Authorize]`).
* `GET /api/account/address` - Get user shipping address (`[Authorize]`).
* `PUT /api/account/address` - Update user shipping address (`[Authorize]`).


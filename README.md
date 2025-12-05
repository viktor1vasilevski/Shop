# Shop API

A RESTful API for managing products built with .NET and Entity Framework Core.

## Prerequisites

- .NET SDK 10.0
- SQL Server or SQL Server LocalDB

## Getting Started

### 1. Configure Database Connection

Update the connection string in `Shop.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ShopDb;Trusted_Connection=True;Encrypt=False"
  }
}
```

### 2. Run Database Migrations

```bash
cd Shop.Api
dotnet ef database update --project ../Shop.Infrastructure
```

### 3. Run the Application

```bash
dotnet run --project Shop.Api
```

The API will be available at `https://localhost:7139`

### 4. Access Swagger Documentation

Navigate to: `https://localhost:7139/swagger/index.html`

## API Endpoints

- `GET /api/Product` - Get all products
- `GET /api/Product/{id}` - Get product by ID
- `POST /api/Product` - Create a new product
- `PUT /api/Product/{id}` - Update a product
- `DELETE /api/Product/{id}` - Delete a product

## Running Tests

```bash
dotnet test
```

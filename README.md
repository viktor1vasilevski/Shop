# Shop

## Project Purpose

This project implements a basic shop backend in C# using .NET. It demonstrates clean architecture principles—including Domain-Driven Design—with clear separation between API, Application, Domain, and Infrastructure layers.

## Architecture Overview

```
Shop/
├── Shop.Api               # The Web API entry point (controllers, routing)
├── Shop.Application       # Application logic (use cases, services)
├── Shop.Domain            # Core domain models and business rules
├── Shop.Infrastructure   # Database implementations and external integrations
├── Shop.Application.Tests # Tests for Application layer
├── Shop.Domain.Tests      # Tests for Domain layer
```

## Installation

1.  Clone the repository or download the zip file
```bash
git clone https://github.com/viktor1vasilevski/Shop.git
```
2.  Change the connection string - In the Api project in the ```appsettings.json``` file enter your server name 
    and name of the database.
    
3.  Go into the Package Manager Console and type: ```Add-Migration init```
4.  When this is done, just type in the Package Manager Console: ```Update-Database```


## Running the Application

```bash
cd Shop.Api
dotnet run
```
or just Run in VS


By default, the API will be accessible at `https://localhost:7139`, and on swagger `https://localhost:7139/swagger/index.html`.


## Design Decisions & Notable Choices

- **Domain rich models:** The domain model is the source of truth, enforcing business rules and valid state through its own behavior.
- **Separation of Concerns:** Each layer has a specific responsibility, making the app maintainable and testable.
- **Unit Testing:** Core business logic and domain models are covered with tests.
- **Extensible:** Infrastructure and Application layers are loosely coupled for easy future improvements.

## Docker

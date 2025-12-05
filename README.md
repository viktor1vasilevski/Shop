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

## Setup

```bash
git clone https://github.com/viktor1vasilevski/Shop.git
cd Shop
dotnet restore
```

## Running the Application

```bash
cd Shop.Api
dotnet run
```


By default, the API will be accessible at `https://localhost:7139`.

## Running the Tests

```bash
dotnet test
```

## Design Decisions & Notable Choices

- **Separation of Concerns:** Each layer has a specific responsibility, making the app maintainable and testable.
- **Unit Testing:** Core business logic is covered with tests.
- **Extensible:** Infrastructure and Application layers are loosely coupled for easy future improvements.

## Contact

Questions? Reach out to [Your Name or GitHub handle].

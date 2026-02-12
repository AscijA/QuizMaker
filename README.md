# QuizMaker API

A robust ASP.NET Core 8 Web API for managing quizzes and questions. It features cursor-based pagination, advanced search, API Key authentication, and a plugin-based export system using MEF.

## Features

* **Quiz Management:** Create, Read, Update, and Delete (CRUD) quizzes.
* **Question Bank:** Manage questions independently or within quizzes.
* **Advanced Search:** Full-text search for questions using EF Core (PostgreSQL `ILike` support).
* **Cursor Pagination:** High-performance pagination for large datasets.
* **Export System:** Plugin-based architecture (MEF) to export quizzes.
* *Current Plugins:* CSV Exporter, PDF Exporter.


* **Security:** API Key Authentication via `X-Api-Key` header.
* **Robust Testing:** Full suite of Integration Tests using `WebApplicationFactory` and xUnit.

## Tech Stack

* **Framework:** .NET 8 (ASP.NET Core)
* **Database:** PostgreSQL (Entity Framework Core)
* **ORM:** EF Core 8
* **Dependency Injection:** Native DI + MEF (Managed Extensibility Framework) for plugins.
* **Logging:** Serilog
* **Testing:** xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing (Integration)

---

## Getting Started

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [PostgreSQL](https://www.postgresql.org/) (Running locally or in Docker)
* IDE (Visual Studio 2022, VS Code, or Rider)

### 1. Configuration

Update `appsettings.json` in `QuizMaker.Api` with your database connection string and API Key:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=QuizMaker;Username=postgres;Password=your_password"
  },
  "Authentication": {
    "ApiKey": "YourSecretKeyHere"
  }
}

```

### 2. Database Migrations

The application uses EF Core migrations. To apply them:

```bash
# Navigate to the API folder
cd QuizMaker.Api

# Create the migration (if you changed the model)
dotnet ef migrations add InitialCreate

# Apply the migration to the DB
dotnet ef database update

```


### 3. Running the Application

```bash
dotnet run --project QuizMaker.Api

```

The API will start at https://localhost:44346.
Swagger UI is available at `/swagger/index.html`.

---

## Running Tests

The project includes a comprehensive Integration Test suite.

**Key Testing Features:**

* **In-Memory Database:** Tests run against a clean In-Memory database for speed and isolation.
* **Migration Skipping:** The test configuration automatically disables SQL migrations to prevent crashes in the In-Memory environment.
* **Isolated Contexts:** Each test class runs with a fresh context.

To run the tests:

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

```

---

## Plugin System (Exports)

The export functionality is built using **MEF (Managed Extensibility Framework)**. This allows adding new export formats (e.g., PDF, JSON) without modifying the core Service code.

### Adding a New Exporter

1. Create a class implementing `IQuizExporter`.
2. Decorate it with `[Export(typeof(IQuizExporter))]`.
3. Implement the logic to return a `byte[]`.

**Example (CSV Exporter):**

```csharp
[Export(typeof(IQuizExporter))]
public class CsvExporter : IQuizExporter 
{
    public string Format => "csv";
    public Task<byte[]> ExportAsync(QuizDetailDto quiz) { ... }
}

```

---

## Authentication

All endpoints (except Swagger) are protected by API Key authentication.

**Header Required:**

```http
X-Api-Key: YourSecretKeyHere

```

*If the key is missing or invalid, the API returns `401 Unauthorized`.*

---
 
## Project Structure

```bash
QuizMaker
├── QuizMaker.Api               # Controllers, Middleware, Entry Point
├── QuizMaker.Application       # Services, Interfaces, DTOs, Business Logic
├── QuizMaker.Domain            # Entities (Quiz, Question)
├── QuizMaker.Infrastructure    # EF Core, Repositories, Migrations, Exporters
└── QuizMaker.IntegrationTests  # xUnit Integration Tests

```
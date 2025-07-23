# CleanArch.AspNetCore

![GitHub last commit](https://img.shields.io/github/last-commit/gasbrieo/dotnet-cleanarch-aspnetcore)
![Sonar Quality Gate](https://img.shields.io/sonar/quality_gate/gasbrieo_dotnet-cleanarch-aspnetcore?server=https%3A%2F%2Fsonarcloud.io)
![Sonar Coverage](https://img.shields.io/sonar/coverage/gasbrieo_dotnet-cleanarch-aspnetcore?server=https%3A%2F%2Fsonarcloud.io)
![NuGet](https://img.shields.io/nuget/v/Gasbrieo.CleanArch.AspNetCore)

A lightweight **ASP.NET Core integration library** for [dotnet-cleanarch](https://github.com/gasbrieo/dotnet-cleanarch), providing:

- ✅ **Result → HTTP mappers**
- ✅ **Minimal API endpoint discovery**
- ✅ **ProblemDetails integration**

Designed to **reduce boilerplate** when exposing Clean Architecture building blocks through ASP.NET Core.

---

## ✨ Features

- ✅ **Map `Result` and `Result<T>` from the domain to proper HTTP responses**
- ✅ **`Validation` errors → `ValidationProblemDetails` (grouped by field)**
- ✅ **`Problem` errors → `ProblemDetails` (single business rule error)**
- ✅ **Automatic Minimal API endpoint registration** via `IEndpoint`
- ✅ **Seamless integration with `dotnet-cleanarch` core errors**

---

## 🧱 Tech Stack

| Layer   | Stack                             |
| ------- | --------------------------------- |
| Runtime | .NET 9                            |
| Package | NuGet                             |
| CI/CD   | GitHub Actions + semantic-release |

---

## 📦 Installation

```bash
dotnet add package Gasbrieo.CleanArch.AspNetCore
```

---

## 🚀 Usage

**1️⃣ Mapping Results to HTTP**

```csharp
app.MapPost("/users", async (CreateUserCommand command, IMediator mediator) =>
{
    Result<UserDto> result = await mediator.Send(command);

    return result.IsSuccess
        ? Results.Ok(result.Value)
        : CustomResults.Problem(result);
});
```

✅ `CustomResults.Problem(result)` automatically maps:

- `Validation` → `400 Bad Request` with **ValidationProblemDetails**
- `Problem` → `400 Bad Request` with **ProblemDetails**
- `NotFound` → `404 Not Found`
- `Conflict` → `409 Conflict`
- `Failure` (default) → `500 Internal Server Error`

---

**2️⃣ Minimal API Endpoint Discovery with API Versioning**

Define endpoints implementing a **versioned interface**, for example `IEndpointV1`:

```csharp
public interface IEndpointV1 : IEndpoint {}

public class UsersEndpoint : IEndpointV1
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id}", (Guid id) =>
        {
            // ...
        });
    }
}
```

Then register & map automatically for that API version:

```csharp
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
var app = builder.Build();

// Register only endpoints for version 1.0
app.MapEndpoints<IEndpointV1>(new ApiVersion(1, 0));
```

This way, you can easily group endpoints by version without manually registering routes one by one.

---

## 🧱 Error Types Integration

This package reuses the same `ErrorType` from **dotnet-cleanarch** and maps them to **ProblemDetails** transparently:

- **Validation** → multiple field errors → `ValidationProblemDetails` (`400 Bad Request`)
- **Problem** → known infrastructure/application issue → `ProblemDetails` (`400 Bad Request`)
- **NotFound** → missing entity/resource → `ProblemDetails` (`404 Not Found`)
- **Conflict** → conflicting state → `ProblemDetails` (`409 Conflict`)
- **Failure** → unknown/unexpected error → `ProblemDetails` (`500 Internal Server Error`)

You define errors once in your **application/domain**, and this package handles the **HTTP mapping** consistently.

---

## 🔄 Releases & Versioning

This project uses **[semantic-release](https://semantic-release.gitbook.io/semantic-release/)** for fully automated versioning:

- **feat:** → minor version bump (0.x.0 → 0.(x+1).0)
- **fix:** → patch version bump (0.0.x → 0.0.(x+1))
- **feat!: / BREAKING CHANGE:** → major version bump (x.0.0 → (x+1).0.0)

Every merge into `main` automatically:

- Updates `CHANGELOG.md`
- Creates a GitHub release
- Publishes a new version to NuGet

See all changes in the [CHANGELOG.md](./CHANGELOG.md).

---

## 🪪 License

This project is licensed under the MIT License – see [LICENSE](LICENSE) for details.

# AGENTS.md - NoviVovi AI Coding Guide

## Project Overview
**NoviVovi** is a visual novel engine backend built with ASP.NET Core 10.0. It manages novel content (chapters, characters, dialogue), provides an API layer, and supports interactive preview sessions with state management. The frontend (not included in scope) consumes this API.

### Technology Stack
- **.NET 10.0** with C# (nullable reference types enabled)
- **MediatR 14.1.0** - CQRS pattern for command/query handling
- **Entity Framework Core 10.0.3** - ORM with PostgreSQL (Npgsql)
- **Riok.Mapperly 5.0.0-next.2** - Source-generated mappers (prefer over AutoMapper)
- **Scalar.AspNetCore** - OpenAPI documentation UI

---

## Architecture: Layered + CQRS

### Core Structure
```
backend/
├── NoviVovi.Api/          # HTTP layer (Controllers, Request/Response DTOs, Mappers)
├── NoviVovi.Application/  # Business logic (CQRS Handlers, Features, Services)
├── NoviVovi.Domain/       # Entities, Value Objects, Domain Rules
└── NoviVovi.Infrastructure/ # Database, Repositories, DbContext
```

### Dependency Flow
**Api → Application → Domain**
**Infrastructure → Domain** (never upward)

---

## Critical Patterns & Conventions

### 1. CQRS with MediatR
Every business operation is a **Command** (mutations) or **Query** (reads). Controllers inject `IMediator` and send requests.

**Pattern:**
```csharp
// In Application layer
public record CreateNovelCommand(string Title) : IRequest<NovelDto>;

public class CreateNovelHandler(INovelRepository repo) : IRequestHandler<CreateNovelCommand, NovelDto>
{
    public async Task<NovelDto> Handle(CreateNovelCommand request, CancellationToken ct)
    {
        var novel = Novel.Create(request.Title, startLabel);
        await repo.AddAsync(novel);
        return _mapper.ToDto(novel);
    }
}

// In Api layer (Controller)
var result = await mediator.Send(new CreateNovelCommand(request.Title));
```

**Key locations:**
- `NoviVovi.Application/*/Features/` - Command/Query records and Handlers
- `NoviVovi.Api/*/Controllers/` - HTTP endpoints

### 2. Mappers: CommandMappers vs ResponseMappers
- **CommandMappers** (`Api/**/CommandMappers/`) - HTTP Request → Application Command (using Mapperly `[Mapper]`)
- **ResponseMappers** (`Api/**/Mappers/`) - Domain/Dto → HTTP Response
- **DtoMappers** (`Application/**/Mappers/`) - Domain → Application DTO

**Mapperly usage (source-generated, compile-time safe):**
```csharp
[Mapper]
public partial class CreateNovelCommandMapper
{
    public partial CreateNovelCommand ToCommand(CreateNovelRequest request);
}
```

Register in `DependencyInjection.cs` as `AddSingleton<>`.

### 3. Domain-Driven Design: Entities & Factory Methods
- All domain entities inherit `Entity` base class (`Domain/Common/Entity.cs`)
- Entity ID is immutable `Guid` (cannot be Guid.Empty)
- **Use factory methods** for construction (e.g., `Novel.Create(...)` not `new Novel()`)
- Domain rules enforced in constructors/methods via `DomainException`

**Example:**
```csharp
public class Novel : Entity
{
    public static Novel Create(string? title, Label startLabel)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");
        return new Novel(Guid.NewGuid(), title, startLabel);
    }
}
```

### 4. Repository Pattern
- Interface in `Application/{Feature}/I{Feature}Repository.cs`
- Implementation in `Infrastructure/{Feature}/Persistence/{Feature}Repository.cs`
- Injected in handlers via `IFeatureRepository`

Example: `INovelRepository` / `NovelRepository`, `ILabelRepository` / `LabelRepository`

### 5. Database Configuration with Fluent API
Entity mappings live in `Infrastructure/{Feature}/Persistence/{Feature}Configuration.cs` (implements `IEntityTypeConfiguration<T>`).

EF Core auto-discovers them via:
```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
```

---

## Preview Feature: Stateful Session Management
The **Preview** feature (`Application/Preview/`) manages interactive playthrough sessions:
- `PreviewSessionStore` - in-memory session storage
- `ScenePlayer` - step-by-step story execution
- `SceneStateDto` - serialized UI state sent to frontend

**Flow:**
1. `StartPreviewCommand` → loads novel, creates session
2. `NextStepCommand` → advances story state
3. `ChooseChoiceCommand` → handles player choices

Sessions are ephemeral (in-memory); consider persistence for larger deployments.

---

## File Organization Rules

### Must-Have Files Per Feature
```
Features/
├── Create/
│   ├── CreateXyzCommand.cs (record + handler in same file or separate)
│   └── CreateXyzHandler.cs
├── Get/
│   ├── GetXyzQuery.cs
│   └── GetXyzHandler.cs
├── Patch/
│   └── PatchXyzCommand.cs & Handler
└── Delete/
    └── DeleteXyzCommand.cs & Handler
```

### API Layer Structure
```
Api/{Feature}/
├── Controllers/
├── Requests/          # Input DTOs (records)
├── Responses/         # Output DTOs
├── CommandMappers/    # Request → Command
└── Mappers/           # Response/DTO mapping
```

---

## Database & Persistence

### Connection String
Defined in `appsettings.json`:
```json
"ConnectionStrings": {
  "NovelDatabase": "Host=...; Database=Novels; ..."
}
```

### Migrations
- Location: `Infrastructure/Persistence/Migrations/`
- Run: `dotnet ef migrations add MigrationName --project Infrastructure`
- Update: `dotnet ef database update --project Infrastructure`

---

## Build & Test Workflow

### Build
```powershell
# Restore & build all projects
dotnet build backend/

# Specific project
dotnet build backend/NoviVovi.Api/NoviVovi.Api.csproj
```

### Run API
```powershell
# From root or Api project directory
dotnet run --project backend/NoviVovi.Api/

# Default port: http://localhost:5136 (per .http file)
# OpenAPI UI: http://localhost:5136/scalar/v1 (Scalar)
```

### Tests
```powershell
# Run all tests
dotnet test tests/

# Specific test project
dotnet test tests/NoviVovi.Domain.Tests/
```

---

## Common AI Tasks & Guidance

### Adding a New Feature (e.g., "Items")
1. **Domain**: Create `Item` entity in `Domain/Items/Item.cs`
2. **Application**: Add `IItemRepository` in `Application/Items/`, then handlers in `Application/Items/Features/{Action}/`
3. **Infrastructure**: Implement `ItemRepository` + `ItemConfiguration` (Fluent API)
4. **API**: Add controller in `Api/Items/Controllers/`, request/response DTOs, mappers
5. **DI**: Register in each layer's `DependencyInjection.cs`

### Modifying Entities
- **Never** directly mutate properties; use behavior methods: `Novel.AddLabel(label)` not `novel.Labels.Add(label)`
- Changes trigger handlers → repository → database

### Testing Repositories
- Use `AppDbContext` with in-memory provider or test database
- Example: `tests/NoviVovi.Infrastructure.Tests/`

---

## Key Files Reference
- **Entry point**: `Api/Program.cs` (middleware, service registration)
- **DI registration**: `Api/DependencyInjection.cs`, `Application/DependencyInjection.cs`, `Infrastructure/DependencyInjection.cs`
- **Entity base**: `Domain/Common/Entity.cs`
- **DbContext**: `Infrastructure/Persistence/AppDbContext.cs`
- **Exception handling**: `Api/Infrastructure/GlobalExceptionHandler.cs`
- **Example domain entity**: `Domain/Novels/Novel.cs`
- **Example CQRS handler**: `Application/Preview/Features/Start/StartPreview.cs`
- **Example API controller**: `Api/Preview/Controllers/PreviewController.cs`

---

## Style & Best Practices
- Use **record types** for DTOs (immutable, concise)
- Use **nullable reference types** (`#nullable enable`)
- Use **Riok.Mapperly** `[Mapper]` for auto-mapping (preferred)
- **DomainException** for domain rule violations
- Keep controllers thin — logic in handlers
- Keep handlers focused on one operation (Single Responsibility)
- Use **async/await** throughout (MediatR, EF Core, repositories)

---

## Debugging Tips
1. **MediatR pipeline**: Check handler registration in `DependencyInjection.cs`
2. **Mapping failures**: Verify `[Mapper]` attributes and partial methods
3. **Database issues**: Check connection string, EF configuration, migrations applied
4. **Session state**: Inspect `PreviewSessionStore` for debug breakpoints


# NoviVovi API Tests

Интеграционные и unit тесты для NoviVovi API.

## Структура тестов

```
tests/
├── NoviVovi.Api.Tests/              # Интеграционные тесты API
│   ├── Infrastructure/              # Тестовая инфраструктура
│   │   ├── TestDatabaseManager.cs   # Управление тестовой БД
│   │   ├── NoviVoviWebApplicationFactory.cs  # WebApplicationFactory
│   │   ├── IntegrationTestBase.cs   # Базовый класс для тестов
│   │   ├── MockStorageService.cs    # Мок для S3
│   │   └── DatabaseCollection.cs    # xUnit collection fixture
│   ├── Novels/                      # Тесты для Novels API
│   ├── Labels/                      # Тесты для Labels API
│   ├── Characters/                  # Тесты для Characters API
│   ├── Images/                      # Тесты для Images API
│   └── Steps/                       # Тесты для Steps API (TODO)
└── NoviVovi.Application.Tests/      # Unit тесты для Application слоя (TODO)
```

## Требования

- .NET 10.0
- PostgreSQL 14+ (локально на localhost:5432)
- Пользователь postgres с паролем postgres

## Запуск тестов

### Все тесты
```bash
dotnet test tests/NoviVovi.Api.Tests/
```

### Конкретный тестовый класс
```bash
dotnet test tests/NoviVovi.Api.Tests/ --filter "FullyQualifiedName~NovelsControllerTests"
```

### Конкретный тест
```bash
dotnet test tests/NoviVovi.Api.Tests/ --filter "FullyQualifiedName~CreateNovel_ValidRequest"
```

## Как работают интеграционные тесты

1. **Изоляция БД**: Каждый запуск тестов создает новую временную БД PostgreSQL
2. **Очистка между тестами**: База очищается перед каждым тестом (TRUNCATE CASCADE)
3. **Реальные HTTP запросы**: Тесты используют `WebApplicationFactory` для создания in-memory сервера
4. **Проверка БД**: Тесты проверяют не только HTTP ответы, но и данные в БД

## Написание новых тестов

### Интеграционный тест

```csharp
[Collection("Database collection")]
public class MyControllerTests(NoviVoviWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task MyTest_Scenario_ExpectedResult()
    {
        // Arrange
        var request = new MyRequest("data");

        // Act
        var response = await PostAsync<MyResponse>("/api/my-endpoint", request);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("expected", response.Value);

        // Verify in database
        var dbRecord = await QuerySingleAsync<dynamic>(
            @"SELECT * FROM ""MyTable"" WHERE ""id"" = @Id",
            new { Id = response.Id });
        
        Assert.NotNull(dbRecord);
    }
}
```

### Helper методы в IntegrationTestBase

- `PostAsync<T>(url, request)` - POST запрос с десериализацией
- `GetAsync<T>(url)` - GET запрос с десериализацией
- `GetListAsync<T>(url)` - GET запрос для списка
- `PatchAsync<T>(url, request)` - PATCH запрос
- `DeleteAsync(url)` - DELETE запрос
- `PostRawAsync(url, request)` - POST без проверки статуса
- `GetRawAsync(url)` - GET без проверки статуса
- `QuerySingleAsync<T>(sql, param)` - SQL запрос (один результат)
- `QueryAsync<T>(sql, param)` - SQL запрос (список)
- `ClearDatabaseAsync()` - Очистка БД

## Покрытие тестами

### ✅ Реализовано (55 тестов)

- **Novels API** (11 тестов): Create, Get, GetAll, Patch, Delete, GetGraph, Export
- **Labels API** (12 тестов): Add, Get, GetAll, Patch, Delete, Cascade
- **Characters API** (11 тестов): Add, Get, GetAll, Patch, Delete, Cascade
- **CharacterStates API** (10 тестов): Add, Get, GetAll, Patch, Delete
- **Images API** (11 тестов): InitiateUpload, Confirm, Get, Patch, Delete, Workflow

### 🚧 В разработке

- **Steps API**: Add/Get/Patch/Delete для всех типов шагов
- **Preview API**: GetScenePreview, session management
- **Application Unit Tests**: Тесты для handlers с моками

## Troubleshooting

### Ошибка подключения к PostgreSQL
Убедитесь, что PostgreSQL запущен и доступен на localhost:5432 с пользователем postgres/postgres.

### Тесты падают с "too many connections"
Это может произойти при параллельном запуске. Используйте:
```bash
dotnet test --parallel none
```

### База данных не очищается
Проверьте, что `ClearDatabaseAsync()` вызывается в `InitializeAsync()` базового класса.

## CI/CD

Для CI/CD рекомендуется использовать Docker контейнер с PostgreSQL:

```yaml
services:
  postgres:
    image: postgres:16
    environment:
      POSTGRES_PASSWORD: postgres
    ports:
      - 5432:5432
```

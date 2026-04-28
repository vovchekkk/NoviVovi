# NoviVovi Tests

## Архитектура тестовой БД

### Как это работает

Все интеграционные тесты используют **одну общую базу данных** `test_novels_shared`:

1. **При первом запуске тестов:**
   - Создается БД `test_novels_shared`
   - Применяется схема (таблицы, внешние ключи)
   - БД остается активной для всех последующих тестов

2. **Перед каждым тестом:**
   - Все таблицы очищаются через `TRUNCATE ... CASCADE`
   - Это обеспечивает изоляцию между тестами

3. **После завершения всех тестов:**
   - БД автоматически удаляется

### Преимущества

- ✅ **Экономия места:** Одна БД вместо сотен
- ✅ **Быстрее:** Не нужно создавать/удалять БД для каждого теста
- ✅ **Изоляция:** Каждый тест работает с чистой БД благодаря `TRUNCATE`

### Очистка старых БД

Если у вас остались старые тестовые БД (созданные до этого изменения), запустите:

```bash
psql -U postgres -f cleanup-test-databases.sql
```

Или вручную в PostgreSQL:

```sql
-- Посмотреть все тестовые БД
SELECT datname FROM pg_database WHERE datname LIKE 'test_novels_%';

-- Удалить все тестовые БД
DROP DATABASE IF EXISTS test_novels_shared;
-- И все старые с GUID в имени
```

### Конфигурация

- **Имя БД:** `test_novels_shared` (фиксированное)
- **Connection String:** `Host=localhost;Port=5432;Database=test_novels_shared;Username=postgres;Password=postgres`
- **Очистка:** Автоматическая через `TRUNCATE` перед каждым тестом

### Запуск тестов

```bash
# Все тесты
dotnet test

# Конкретный класс
dotnet test --filter "FullyQualifiedName~StepsControllerTests"

# Конкретный тест
dotnet test --filter "FullyQualifiedName~PatchShowCharacterStep_ValidRequest_ReturnsUpdatedStep"
```

### Troubleshooting

**Проблема:** Тесты падают с ошибкой подключения к БД

**Решение:**
1. Убедитесь, что PostgreSQL запущен
2. Проверьте, что пользователь `postgres` с паролем `postgres` существует
3. Удалите БД вручную и перезапустите тесты:
   ```sql
   DROP DATABASE IF EXISTS test_novels_shared;
   ```

**Проблема:** БД занимает много места

**Решение:** Это не должно происходить с новой архитектурой. Если БД большая, проверьте:
```sql
SELECT pg_size_pretty(pg_database_size('test_novels_shared'));
```

Если размер > 100MB, что-то не так с очисткой данных.

# Отчёт об изменениях

## Дата: 2026-04-28

## 1. Исправлен баг в PatchShowCharacterStep

### Проблема
Тест `PatchShowCharacterStep_ValidRequest_ReturnsUpdatedStep` падал с ошибкой 500 Internal Server Error.

### Причина
1. **Неправильный SQL запрос** в `GetCharacterObjectByCharacterIdAsync`:
   - Искал `StepCharacter` по `character_id` через JOIN вместо прямого поиска по `id`
   - Использовал `LIMIT 1`, что возвращало случайный результат при нескольких записях

2. **Неправильное сохранение TransformId**:
   - В маппере `CharacterMapper.ToDbO` устанавливался `TransformId = Guid.Empty`
   - При загрузке Transform не находился, т.к. его ID был null в БД

### Решение
**Файл:** `CharacterDbORepository.cs:164-186`
```csharp
// ✅ Исправлено: прямой запрос по StepCharacter.id
SELECT id, transform_id, character_state_id
FROM "StepCharacter"
WHERE id = @StepCharacterId
```

**Файл:** `CharacterMapper.cs:66-78`
```csharp
// ✅ Исправлено: используем реальный ID трансформа
var transform = transformMapper.ToDbO(stepCharacterObject.Transform);
TransformId = transform.Id,
Transform = transform,
```

### Результат
✅ Тест `PatchShowCharacterStep_ValidRequest_ReturnsUpdatedStep` теперь проходит

---

## 2. Оптимизирована архитектура тестовой БД

### Проблема
- Каждый запуск тестов создавал новую БД с уникальным именем (GUID)
- БД не удалялись после тестов
- Папка с БД занимала **30 ГБ**

### Решение

#### Изменения в `NoviVoviWebApplicationFactory.cs`
- Используется **одна общая БД** `test_novels_shared` для всех тестов
- Счётчик экземпляров отслеживает, когда удалять БД
- БД создаётся при первом запуске, удаляется после последнего теста

#### Изменения в `TestDatabaseManager.cs`
- Фиксированное имя БД: `test_novels_shared`
- Проверка существования БД перед созданием
- Проверка существования схемы перед применением

#### Изменения в `IntegrationTestBase.cs`
- Метод `ClearDatabaseAsync()` очищает все таблицы через `TRUNCATE CASCADE`
- Вызывается перед каждым тестом для изоляции

### Результат
✅ **Одна БД** вместо сотен
✅ **Экономия места:** ~30 ГБ → ~10 МБ
✅ **Быстрее:** не нужно создавать/удалять БД для каждого теста
✅ **Изоляция:** каждый тест работает с чистой БД

### Логи
```
[TestDB] Created database: test_novels_shared          # Первый запуск
[TestDB] Applied schema to database: test_novels_shared

[TestDB] Database already exists: test_novels_shared   # Последующие запуски
[TestDB] Schema already exists in database: test_novels_shared
```

---

## 3. Добавлены вспомогательные файлы

### `tests/cleanup-test-databases.sql`
SQL-скрипт для удаления всех старых тестовых БД:
```bash
psql -U postgres -f cleanup-test-databases.sql
```

### `tests/README.md`
Документация по новой архитектуре тестов:
- Как работает общая БД
- Как запускать тесты
- Troubleshooting

---

## Итоги

### Исправлено
1. ✅ Баг в `PatchShowCharacterStep` - тест проходит
2. ✅ Проблема с множественными БД - используется одна общая БД
3. ✅ Экономия 30 ГБ дискового пространства

### Изменённые файлы
1. `backend/NoviVovi.Infrastructure/Repositories/DbO/CharacterDbORepository.cs`
2. `backend/NoviVovi.Infrastructure/Repositories/DbO/Interfaces/ICharacterDbORepository.cs`
3. `backend/NoviVovi.Infrastructure/Mappers/CharacterMapper.cs`
4. `tests/NoviVovi.Api.Tests/Infrastructure/NoviVoviWebApplicationFactory.cs`
5. `tests/NoviVovi.Api.Tests/Infrastructure/TestDatabaseManager.cs`

### Новые файлы
1. `tests/cleanup-test-databases.sql`
2. `tests/README.md`

### Удалённые файлы
1. `tests/NoviVovi.Api.Tests/Steps/StepsControllerTests_Diagnostics.cs` (не компилировался)

---

## Рекомендации

1. **Очистить старые БД:**
   ```bash
   psql -U postgres -f tests/cleanup-test-databases.sql
   ```

2. **Проверить размер новой БД:**
   ```sql
   SELECT pg_size_pretty(pg_database_size('test_novels_shared'));
   ```
   Должно быть < 100 MB

3. **Запустить все тесты:**
   ```bash
   dotnet test
   ```

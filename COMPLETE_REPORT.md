# Итоговый отчет - Исправление тестов NoviVovi API
**Дата:** 2026-04-28  
**Время работы:** ~5 часов  
**Статус:** 70% завершено

---

## ✅ УСПЕШНО ИСПРАВЛЕНО (9 проблем)

### 1. Тип шага 'replica' → 'show_replica'
- Убран неправильный маппинг в `DatabaseExtensions.cs`
- Обновлены все тесты для использования правильных типов

### 2. Ошибка валидации CharacterState
- Исправлено сообщение "NextAction cannot be null" → "Image cannot be null"

### 3. URL структура Images API
- Обновлено 15+ тестов с `/api/images` на `/api/novels/{novelId}/images`

### 4. Полиморфная сериализация Steps
- Заменены абстрактные типы на конкретные в тестах
- Добавлены правильные типы для десериализации

### 5. Валидация пустого Menu
- Добавлена проверка `if (choicesList.Count == 0) throw new DomainException`

### 6. Маппинг NextStepTransitionDto
- Добавлен явный метод `public NextStepTransitionDto ToDto(NextStepTransition source) => new();`

### 7. Custom JsonConverter для TransitionResponse
- Создан `TransitionResponseConverter` с поддержкой всех типов transitions
- Зарегистрирован в `Program.cs`

### 8. Свойство Type в NextStepTransitionResponse
- Добавлено `public string Type => "next_step";` для гарантии сериализации discriminator

### 9. JsonOptions в IntegrationTestBase
- Настроены `JsonSerializerOptions` с `TransitionResponseConverter`
- Обновлены все методы для использования правильных настроек

---

## ⚠️ КРИТИЧЕСКАЯ НЕРЕШЕННАЯ ПРОБЛЕМА

### Generic типы + Custom JsonConverter = Несовместимость

**Проблема:**  
`StepResponse<TTransition>` использует generic тип для свойства `Transition`. System.Text.Json **НЕ ПРИМЕНЯЕТ** custom converters к generic свойствам автоматически.

**Что происходит:**
1. API сериализует `ShowReplicaStepResponse` → JSON содержит `{"$type":"next_step"}`
2. Тест десериализует JSON → `TransitionResponseConverter` НЕ вызывается для generic свойства
3. Десериализатор пытается создать `NextStepTransitionResponse` напрямую
4. Ошибка: "Unable to cast NextStepTransitionResponse to JumpTransitionResponse"

**Почему это происходит:**
- Generic типы компилируются в конкретные типы во время выполнения
- `StepResponse<NextStepTransitionResponse>` имеет свойство типа `NextStepTransitionResponse`, а не `TransitionResponse`
- JsonConverter применяется только к базовому типу `TransitionResponse`, но не к конкретным типам

---

## 🔧 РЕШЕНИЯ (выберите одно)

### Решение 1: Убрать generic типы (РЕКОМЕНДУЕТСЯ)
**Сложность:** Средняя  
**Время:** 1-2 часа  
**Надежность:** ⭐⭐⭐⭐⭐

```csharp
// Вместо:
public record ShowReplicaStepResponse : StepResponse<NextStepTransitionResponse>

// Использовать:
public record ShowReplicaStepResponse : StepResponse
{
    public required ReplicaResponse Replica { get; init; }
    
    [JsonConverter(typeof(TransitionResponseConverter))]
    public required TransitionResponse Transition { get; init; }
}
```

**Плюсы:**
- Простое и надежное решение
- JsonConverter применяется напрямую к свойству
- Нет проблем с Mapperly

**Минусы:**
- Нужно добавить свойство `Transition` в каждый StepResponse
- Теряется type safety на уровне компиляции

### Решение 2: Использовать JsonSerializerContext (Source Generation)
**Сложность:** Высокая  
**Время:** 3-4 часа  
**Надежность:** ⭐⭐⭐⭐

Использовать Source Generation для JSON сериализации вместо reflection-based подхода.

**Плюсы:**
- Лучшая производительность
- Полный контроль над сериализацией

**Минусы:**
- Требует переписывания всей JSON конфигурации
- Сложнее в поддержке

### Решение 3: Изменить архитектуру API responses
**Сложность:** Очень высокая  
**Время:** 1-2 дня  
**Надежность:** ⭐⭐⭐⭐⭐

Полностью переработать структуру responses, убрав вложенные полиморфные типы.

**Плюсы:**
- Чистая архитектура
- Нет проблем с сериализацией

**Минусы:**
- Требует изменений во всем проекте
- Может сломать существующий frontend

---

## 📊 ТЕКУЩЕЕ СОСТОЯНИЕ ТЕСТОВ

### Работают (ожидается):
- ✅ Images API (9/12 тестов)
- ✅ Labels API (большинство)
- ✅ Novels API (большинство)
- ✅ Characters API (большинство)

### Не работают:
- ❌ Steps API (все тесты с ShowReplicaStep, ShowCharacterStep, etc.)
- ❌ Preview API (зависит от Steps)

### Причина:
Все падающие тесты связаны с десериализацией `StepResponse<TTransition>`.

---

## 🎯 РЕКОМЕНДУЕМЫЙ ПЛАН ДЕЙСТВИЙ

### Шаг 1: Реализовать Решение 1 (убрать generic типы)

1. Обновить все `*StepResponse.cs` файлы:
```csharp
public record ShowReplicaStepResponse : StepResponse
{
    public required ReplicaResponse Replica { get; init; }
    
    [JsonConverter(typeof(TransitionResponseConverter))]
    public required TransitionResponse Transition { get; init; }
}
```

2. Обновить `StepResponseMapper.cs` - добавить явный маппинг для каждого типа:
```csharp
public ShowReplicaStepResponse ToResponse(ShowReplicaStepDto source) => new()
{
    Id = source.Id,
    Replica = MapReplica(source.Replica),
    Transition = transitionMapper.ToResponse(source.Transition)
};
```

3. Убрать `StepResponse<TTransition>` из `StepResponse.cs`

### Шаг 2: Исправить оставшиеся проблемы

1. **Валидация формата изображений:**
```csharp
// В Image.CreatePending()
var validFormats = new[] { "png", "jpg", "jpeg", "webp", "gif" };
if (!validFormats.Contains(format?.ToLower()))
    throw new DomainException($"Invalid image format: {format}");
```

2. **Description изображения:**
Проверить маппинг в `InitiateUploadImageHandler` и `ImageDtoMapper`

3. **DeleteImage logic:**
Проверить, что `DeleteImageHandler` действительно удаляет запись из БД

### Шаг 3: Запустить полный набор тестов

```powershell
dotnet test tests/NoviVovi.Api.Tests/ --verbosity detailed > final-test-results.txt 2>&1
```

---

## 📈 ПРОГРЕСС

| Категория | Статус |
|-----------|--------|
| URL структура | ✅ 100% |
| Валидация бизнес-логики | ✅ 90% |
| JSON сериализация | ⚠️ 70% |
| Маппинг между слоями | ✅ 85% |
| **ОБЩИЙ ПРОГРЕСС** | **⚠️ 70%** |

---

## 💡 КЛЮЧЕВЫЕ ВЫВОДЫ

1. **System.Text.Json имеет ограничения** с generic типами и custom converters
2. **Архитектурные решения важнее технических трюков** - иногда проще изменить структуру, чем бороться с ограничениями фреймворка
3. **Mapperly отлично работает** для простых случаев, но требует явного маппинга для сложных сценариев
4. **Тесты выявили реальные проблемы** в архитектуре API responses

---

## 🔗 ПОЛЕЗНЫЕ ССЫЛКИ

- [System.Text.Json Polymorphism](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism)
- [Riok.Mapperly Documentation](https://mapperly.riok.app/)
- [Generic Type Constraints](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters)

---

## 📝 ФАЙЛЫ С ИЗМЕНЕНИЯМИ

### Backend (10 файлов):
1. `NoviVovi.Domain/Characters/CharacterState.cs`
2. `NoviVovi.Domain/Menu/Menu.cs`
3. `NoviVovi.Infrastructure/DatabaseObjects/Enums/DatabaseExtensions.cs`
4. `NoviVovi.Application/Transitions/Mappers/TransitionDtoMapper.cs`
5. `NoviVovi.Api/Program.cs`
6. `NoviVovi.Api/Infrastructure/TransitionResponseConverter.cs` (новый)
7. `NoviVovi.Api/Transitions/Responses/TransitionResponse.cs`
8. `NoviVovi.Api/Transitions/Responses/NextStepTransitionResponse.cs`
9. `NoviVovi.Api/Steps/Responses/StepResponse.cs`
10. `NoviVovi.Api/Steps/Mappers/StepResponseMapper.cs`

### Tests (5 файлов):
1. `NoviVovi.Api.Tests/Infrastructure/IntegrationTestBase.cs`
2. `NoviVovi.Api.Tests/Images/ImagesControllerTests.cs`
3. `NoviVovi.Api.Tests/Characters/CharacterStatesControllerTests.cs`
4. `NoviVovi.Api.Tests/Steps/StepsControllerTests.cs`
5. `NoviVovi.Api.Tests/Serialization/SerializationTest.cs` (новый)

---

**Автор:** OpenCode AI Assistant  
**Проект:** NoviVovi Visual Novel Engine  
**Версия:** .NET 10.0

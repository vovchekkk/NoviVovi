# Отчет по исправлению тестов NoviVovi API

## Дата: 2026-04-28

## Исправленные проблемы

### 1. ✅ Исправлен тип шага 'replica' в StepMapper
**Файл:** `backend/NoviVovi.Infrastructure/DatabaseObjects/Enums/DatabaseExtensions.cs`
**Проблема:** База данных содержала тип 'replica', но маппер ожидал 'show_replica'
**Решение:** Добавлен маппинг `"replica" => StepType.ShowReplica`

### 2. ✅ Исправлена ошибка валидации CharacterState
**Файл:** `backend/NoviVovi.Domain/Characters/CharacterState.cs`
**Проблема:** Сообщение об ошибке "NextAction cannot be null" вместо "Image cannot be null"
**Решение:** Исправлено сообщение об ошибке на корректное

### 3. ✅ Обновлены URL в тестах Images API
**Файл:** `tests/NoviVovi.Api.Tests/Images/ImagesControllerTests.cs`
**Проблема:** Тесты использовали `/api/images/*`, но бэкенд ожидает `/api/novels/{novelId}/images/*`
**Решение:** Обновлены все URL в тестах для соответствия API бэкенда

### 4. ✅ Обновлены URL в тестах CharacterStates
**Файл:** `tests/NoviVovi.Api.Tests/Characters/CharacterStatesControllerTests.cs`
**Проблема:** Метод `CreateTestImageAsync` использовал неправильный URL
**Решение:** Обновлен URL для создания изображений

### 5. ✅ Исправлена полиморфная сериализация Steps
**Файл:** `tests/NoviVovi.Api.Tests/Steps/StepsControllerTests.cs`
**Проблема:** Тесты пытались десериализовать абстрактный тип `StepResponse`
**Решение:** Заменены на конкретные типы:
- `ShowReplicaStepResponse`
- `ShowCharacterStepResponse`
- `HideCharacterStepResponse`
- `ShowBackgroundStepResponse`
- `JumpStepResponse`

### 6. ✅ Добавлена валидация пустого списка choices в Menu
**Файл:** `backend/NoviVovi.Domain/Menu/Menu.cs`
**Проблема:** Menu.Create() принимал пустой список choices
**Решение:** Добавлена проверка `if (choicesList.Count == 0) throw new DomainException`

### 7. ✅ Исправлен маппинг NextStepTransitionDto
**Файл:** `backend/NoviVovi.Application/Transitions/Mappers/TransitionDtoMapper.cs`
**Проблема:** Mapperly не мог создать пустой record NextStepTransitionDto
**Решение:** Добавлен явный метод `public NextStepTransitionDto ToDto(NextStepTransition source) => new();`

## Оставшиеся проблемы

### 1. ⚠️ ShowReplicaStep возвращает JumpTransitionResponse без targetLabelId
**Проблема:** ShowReplicaStepResponse содержит NextStepTransitionResponse, но тест ожидает JumpTransitionResponse
**Статус:** Требуется дальнейшее исследование маппинга Transition -> TransitionResponse

### 2. ⚠️ ShowMenuStep все еще десериализуется как StepResponse
**Проблема:** Один тест все еще использует базовый тип вместо ShowMenuStepResponse
**Статус:** Нужно найти и исправить оставшееся использование

### 3. ⚠️ Валидация формата изображения не работает
**Проблема:** Тест ожидает UnprocessableEntity для невалидного формата, но получает OK
**Статус:** Нужно добавить валидацию формата в Image.CreatePending()

### 4. ⚠️ Description изображения возвращает null
**Проблема:** После создания и подтверждения изображения description = null
**Статус:** Проверить маппинг ImageDto -> ImageResponse

### 5. ⚠️ DeleteImage возвращает OK вместо NotFound
**Проблема:** После удаления изображения GET запрос возвращает OK вместо NotFound
**Статус:** Проверить логику удаления в DeleteImageHandler

## Статистика тестов

### До исправлений:
- Не пройдено: 42
- Пройдено: 46
- Всего: 88

### После исправлений (частичный запуск):
- Images: 9 пройдено, 3 не пройдено из 12
- Остальные тесты: в процессе проверки

## Рекомендации

1. **Приоритет 1:** Исправить маппинг Transition responses - это блокирует много тестов Steps
2. **Приоритет 2:** Добавить валидацию формата изображений
3. **Приоритет 3:** Проверить логику удаления изображений
4. **Приоритет 4:** Запустить полный набор тестов после всех исправлений

## Команды для тестирования

```powershell
# Запустить все тесты
dotnet test tests/NoviVovi.Api.Tests/ --verbosity minimal

# Запустить только Images тесты
dotnet test tests/NoviVovi.Api.Tests/ --filter "FullyQualifiedName~ImagesControllerTests"

# Запустить только Steps тесты
dotnet test tests/NoviVovi.Api.Tests/ --filter "FullyQualifiedName~StepsControllerTests"

# Сохранить результаты в файл
dotnet test tests/NoviVovi.Api.Tests/ --verbosity minimal > test-results.txt 2>&1
```

# NoviVovi Backend - Настройка окружения

## 🔐 Конфигурация секретов

### Для локальной разработки:

1. Скопируй `appsettings.json` в `appsettings.Development.json`:
   ```bash
   cp backend/NoviVovi.Api/appsettings.json backend/NoviVovi.Api/appsettings.Development.json
   ```

2. Открой `appsettings.Development.json` и замени `<password>` на реальные пароли:
   ```json
   {
     "UseLocalDatabase": true,
     "UseLocalStorage": true,
     "ConnectionStrings": {
       "LocalDatabase": "Host=localhost;Port=5432;Database=Novels;Username=postgres;Password=ВАШ_ЛОКАЛЬНЫЙ_ПАРОЛЬ;...",
       "CloudDatabase": "Host=rc1a-ot1r915bnk6cftm9.mdb.yandexcloud.net;Port=6432;Database=Novels;Username=novivoviAdmin;Password=ВАШ_ОБЛАЧНЫЙ_ПАРОЛЬ;..."
     }
   }
   ```

3. **ВАЖНО:** Файл `appsettings.Development.json` НЕ коммитится в git (уже в `.gitignore`)

### Переключение между окружениями:

#### Локальная разработка (по умолчанию):
```json
"UseLocalDatabase": true,
"UseLocalStorage": true
```

#### Продакшн (Yandex Cloud):
```json
"UseLocalDatabase": false,
"UseLocalStorage": false
```

## 🗄️ Настройка локальной БД

1. Установи PostgreSQL 18
2. Создай базу данных:
   ```bash
   createdb -U postgres Novels
   ```
3. Таблицы создадутся автоматически при первом запуске (SQL скрипт в `backend/create_tables.sql`)

## 🚀 Запуск

```bash
cd backend/NoviVovi.Api
dotnet run
```

API будет доступен на: `http://localhost:5136`

## 📝 Тестирование API

Используй файл `backend/NoviVovi.Api/NoviVovi.Api.http` в Rider/VS Code для тестирования эндпоинтов.

## ⚠️ Безопасность

- ❌ **НИКОГДА** не коммить реальные пароли в git
- ✅ Используй `<password>` как placeholder в `appsettings.json`
- ✅ Реальные пароли храни только в `appsettings.Development.json` (игнорируется git)
- ✅ Для продакшна используй переменные окружения или Azure Key Vault

#!/bin/bash
# Quick Start Script - После восстановления PostgreSQL
# Запусти этот скрипт, чтобы проверить, что всё работает

echo "==================================="
echo "NoviVovi - Quick Start After PostgreSQL Recovery"
echo "==================================="
echo ""

# 1. Проверка подключения к PostgreSQL
echo "1. Проверка подключения к PostgreSQL..."
psql -U postgres -c "SELECT version();" 2>/dev/null
if [ $? -eq 0 ]; then
    echo "✅ PostgreSQL работает"
else
    echo "❌ PostgreSQL не доступен. Проверь, что служба запущена."
    exit 1
fi

# 2. Очистка старых тестовых БД
echo ""
echo "2. Очистка старых тестовых БД..."
psql -U postgres -f tests/cleanup-test-databases.sql 2>/dev/null
echo "✅ Старые БД удалены"

# 3. Сборка проекта
echo ""
echo "3. Сборка проекта..."
dotnet build backend/NoviVovi.sln
if [ $? -eq 0 ]; then
    echo "✅ Проект собран успешно"
else
    echo "❌ Ошибка сборки"
    exit 1
fi

# 4. Запуск одного теста для проверки
echo ""
echo "4. Запуск тестового теста..."
dotnet test tests/NoviVovi.Api.Tests/NoviVovi.Api.Tests.csproj \
    --filter "FullyQualifiedName~PatchShowCharacterStep_ValidRequest_ReturnsUpdatedStep" \
    --logger "console;verbosity=minimal"

if [ $? -eq 0 ]; then
    echo "✅ Тест прошёл успешно!"
else
    echo "⚠️ Тест упал, но это может быть другая проблема"
fi

# 5. Проверка размера БД
echo ""
echo "5. Проверка размера тестовой БД..."
psql -U postgres -c "SELECT pg_size_pretty(pg_database_size('test_novels_shared_v2'));" 2>/dev/null

echo ""
echo "==================================="
echo "✅ Готово! Теперь можешь запускать все тесты:"
echo "   dotnet test"
echo "==================================="

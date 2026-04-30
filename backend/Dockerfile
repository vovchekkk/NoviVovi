FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Указываем, что внутри контейнера будет приложение на .NET 8
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .

# Собираем проект
RUN dotnet publish "NoviVovi.Api/NoviVovi.Api.csproj" -c Release -o /app/publish

# Финальный образ: берем только результат сборки
FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .

RUN apt-get update && apt-get install -y ca-certificates && update-ca-certificates

# Точка входа: какая команда выполнится при запуске контейнера
ENTRYPOINT ["dotnet", "NoviVovi.Api.dll"]
# Lab9 - Конфигурирование ASP.NET Core приложения

## Цель занятия
Освоить процесс конфигурирования веб-приложений на платформе ASP.NET Core для эффективного управления параметрами приложения.

## Что реализовано
1. Созданы файлы конфигурации для разных сред:
- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Testing.json`
- `appsettings.Production.json`

2. Используются разные источники конфигурации:
- JSON-файлы (`appsettings*.json`)
- переменные среды (`ASPNETCORE_ENVIRONMENT`, `ASPNETCORE_URLS`, `AppSettings__Owner`, `LAB9_AppSettings__Owner`)
- параметры веб-сервера (`Urls`/`ASPNETCORE_URLS`)
- параметры командной строки (например `--Greeting:Message="..."`)
- in-memory источник (`ConfigMeta:InMemorySource`)

3. Настроена обработка параметров в коде:
- используется Options pattern (`AppSettingsOptions`, `GreetingOptions`, `FeatureFlagsOptions`)
- endpoint `/config` показывает эффективные значения после объединения источников
- endpoint `/config/providers` показывает подключенные provider'ы
- endpoint `/diagnostics` зависит от feature flag `FeatureFlags:EnableDiagnostics`

4. Подготовлена проверка работы для разных сред через запуск с разными `ASPNETCORE_ENVIRONMENT`.

## Полезные endpoint'ы
- `/` - текущая среда и приветствие
- `/config` - итоговая конфигурация
- `/config/providers` - список источников конфигурации
- `/diagnostics` - диагностика (доступна при `EnableDiagnostics=true`)

## Примеры запуска
### Development
```bash
ASPNETCORE_ENVIRONMENT=Development dotnet run --no-launch-profile --project Lab9
```

### Testing
```bash
ASPNETCORE_ENVIRONMENT=Testing dotnet run --no-launch-profile --project Lab9
```

### Production
```bash
ASPNETCORE_ENVIRONMENT=Production dotnet run --no-launch-profile --project Lab9
```

## Примеры переопределения конфигурации
### Через переменные среды
```bash
ASPNETCORE_ENVIRONMENT=Testing \
AppSettings__Owner="Owner from ENV" \
ASPNETCORE_URLS=http://localhost:5299 \
dotnet run --no-launch-profile --project Lab9
```

### Через параметры командной строки
```bash
dotnet run --no-launch-profile --project Lab9 -- --Greeting:Message="Hello from CLI"
```

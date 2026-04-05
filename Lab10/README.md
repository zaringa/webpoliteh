# Lab10 - Логирование в ASP.NET Core

## Цель занятия
Ознакомиться с методами логирования и их применением в веб-приложениях на платформе ASP.NET Core.

## Что реализовано
1. Настроено логирование с несколькими провайдерами:
- Console (`AddSimpleConsole`)
- Debug (`AddDebug`)
- EventSource (`AddEventSourceLogger`)
- пользовательский файловый провайдер (`FileLoggerProvider`)
- пользовательский провайдер в базу данных SQLite (`DatabaseLoggerProvider`)

2. Применены разные уровни логирования для компонентов:
- `Lab10.Services` - более подробные логи
- `Lab10.Endpoints` - отдельный уровень
- `Microsoft*` - ограничены до предупреждений/информационных по среде

3. Реализованы пользовательские обработчики/форматы:
- файл: кастомный строковый формат с временем, категорией, EventId, scope и exception
- база данных: структурированная запись в таблицу `ApplicationLogs`

4. Подготовлено тестирование логов через endpoint'ы:
- генерация тестовых логов
- чтение последних записей из файла
- чтение последних записей из базы

## Файлы логов
- Файл: `Lab10/Logs/lab10-file.log`
- БД: `Lab10/Logs/lab10-logs.db`

## Запуск
```bash
dotnet run --no-launch-profile --project Lab10 --urls http://localhost:5310
```

## Проверка
1. Сгенерировать логи:
```bash
curl -X POST "http://localhost:5310/logs/generate?count=5&withError=true"
```

2. Проверить файл:
```bash
curl "http://localhost:5310/logs/file?lines=20"
```

3. Проверить базу:
```bash
curl "http://localhost:5310/logs/database?take=20"
```

4. Проверить уровни по компонентам:
```bash
curl -X POST "http://localhost:5310/orders/10?fail=false"
curl -X POST "http://localhost:5310/orders/10?fail=true"
```

## Формат сдачи
- Папка проекта: `Lab10`
- Исходный код пользовательских провайдеров логирования
- Файлы конфигурации (`appsettings.json`, `appsettings.Development.json`)
- Краткий отчёт по endpoint'ам и результатам проверки (этот README)

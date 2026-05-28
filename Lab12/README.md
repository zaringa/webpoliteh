# Lab12 - Обработка ошибок в ASP.NET Core

## Цель
Ознакомиться с основными способами обработки ошибок в ASP.NET Core Web API.

## Что реализовано
1. Создан отдельный проект `Lab12` (ASP.NET Core Web API + EF Core + SQLite).
2. Обработка ошибок бизнес-логики через `ModelState`:
- в `ProductsController` проверяется правило `DiscountPrice < Price`;
- дополнительное правило: SKU с префиксом `VIP-` требует `Price >= 100`.
3. Фильтр ошибок базы данных:
- `DatabaseExceptionFilter` перехватывает `DbUpdateException` и `DbUpdateConcurrencyException`;
- возвращает `ProblemDetails` со статусом `409`.
4. Глобальная обработка необработанных исключений:
- `GlobalExceptionMiddleware` перехватывает непойманные исключения;
- возвращает `ProblemDetails` со статусом `500` и `traceId`.
5. Подготовлены тестовые endpoint'ы и HTTP-запросы для Swagger/Postman.

## Структура
- `Program.cs` - регистрация EF Core, фильтра, middleware, Swagger.
- `Data/AppDbContext.cs` - контекст БД и уникальный индекс `Sku`.
- `Models/Product.cs` - сущность.
- `Controllers/ProductsController.cs` - CRUD + `ModelState` бизнес-валидация.
- `Filters/DatabaseExceptionFilter.cs` - фильтр ошибок БД.
- `Middleware/GlobalExceptionMiddleware.cs` - глобальная обработка исключений.
- `Controllers/ErrorsController.cs` - endpoint для демонстрации необработанной ошибки.
- `Migrations/*` - миграция `InitialCreate`.

## Запуск
```bash
cd /home/zarina/Данные/webpoliteh/Lab12
dotnet restore
dotnet build
dotnet run --no-launch-profile --urls http://localhost:5012
```

Swagger:
`http://localhost:5012/swagger`

## Сценарий показа на защите
1. Показать `Program.cs`:
- подключение `AddControllers` + глобальный фильтр `DatabaseExceptionFilter`;
- `app.UseMiddleware<GlobalExceptionMiddleware>()`;
- `db.Database.Migrate()`.
2. Показать `ProductsController.cs` и объяснить `ModelState`-валидацию бизнес-логики.
3. Показать `DatabaseExceptionFilter.cs`.
4. Показать `GlobalExceptionMiddleware.cs`.
5. В Swagger выполнить тесты:
- успешный `POST /api/products`;
- `POST /api/products` с `discountPrice >= price` (получить `400`);
- `POST /api/products` с дублирующимся `sku` (получить `409`);
- `GET /api/errors/unhandled` (получить `500`).

## Примеры JSON для теста

Успешное создание:
```json
{
  "name": "Mouse Logitech",
  "sku": "MOU-001",
  "price": 1200.00,
  "discountPrice": 1000.00,
  "stockQuantity": 15
}
```

Бизнес-ошибка (`ModelState`):
```json
{
  "name": "Broken Discount",
  "sku": "MOU-002",
  "price": 1000.00,
  "discountPrice": 1200.00,
  "stockQuantity": 5
}
```

Ошибка БД (дублирование `sku`):
```json
{
  "name": "Duplicate SKU",
  "sku": "MOU-001",
  "price": 900.00,
  "discountPrice": 800.00,
  "stockQuantity": 7
}
```

## Что добавить в PDF-отчёт
1. Цель, задачи, стек.
2. Скрины ключевого кода (`ModelState`, фильтр БД, middleware).
3. Скрины Swagger с 3 типами ошибок (`400`, `409`, `500`).
4. Краткие выводы и сложности.

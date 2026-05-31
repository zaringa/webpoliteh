# Lab11 - EF Core в ASP.NET Core Web API

## Цель
Ознакомиться с использованием ORM Entity Framework Core в ASP.NET Core Web API и реализовать полный CRUD для связанных сущностей.

## Что реализовано
1. Проект ASP.NET Core Web API (`net8.0`).
2. Три связанные сущности:
- `Author` (автор)
- `Category` (категория)
- `Book` (книга, содержит `AuthorId` и `CategoryId`)
3. Data Access Layer на EF Core (Code First):
- `AppDbContext`
- SQLite база `lab11.db`
- миграция `InitialCreate`
4. Полный CRUD API для:
- `/api/authors`
- `/api/categories`
- `/api/books`
5. Проверка через Swagger и `curl`.

## Ключевые файлы
- `Program.cs` - регистрация EF Core, Swagger, применение миграций.
- `Data/AppDbContext.cs` - конфигурация сущностей и связей.
- `Models/*.cs` - сущности БД.
- `Controllers/*.cs` - CRUD-методы.
- `Migrations/*` - миграция Code First.
- `appsettings.json` - строка подключения SQLite.

## Запуск
```bash
cd /home/zarina/Данные/webpoliteh/Lab11
dotnet restore
dotnet build
dotnet run --no-launch-profile --urls http://localhost:5011
```

Swagger:
`http://localhost:5011/swagger`

## Сценарий показа на защите
1. Показать структуру проекта (`Models`, `Data`, `Controllers`, `Migrations`).
2. Открыть `Data/AppDbContext.cs` и объяснить связи:
- `Book -> Author` (many-to-one)
- `Book -> Category` (many-to-one)
3. Открыть `Program.cs`:
- подключение `UseSqlite`
- `db.Database.Migrate()`
4. Открыть `Migrations/20260428004503_InitialCreate.cs` как доказательство Code First.
5. Открыть Swagger и выполнить CRUD в таком порядке:
- `POST /api/authors`
- `POST /api/categories`
- `POST /api/books`
- `GET /api/books`
- `PUT /api/books/{id}`
- `DELETE /api/books/{id}`
- `GET /api/books` (должен вернуть пустой массив или список без удалённой записи)

## Быстрые команды для live-демо в терминале
```bash
curl -X POST http://localhost:5011/api/authors \
  -H "Content-Type: application/json" \
  -d '{"name":"Ray Bradbury","email":"bradbury@example.com"}'

curl -X POST http://localhost:5011/api/categories \
  -H "Content-Type: application/json" \
  -d '{"name":"Science Fiction","description":"Fiction with scientific ideas"}'

curl -X POST http://localhost:5011/api/books \
  -H "Content-Type: application/json" \
  -d '{"title":"Fahrenheit 451","year":1953,"price":20.00,"authorId":1,"categoryId":1}'

curl http://localhost:5011/api/books

curl -X PUT http://localhost:5011/api/books/1 \
  -H "Content-Type: application/json" \
  -d '{"title":"Fahrenheit 451 (Updated)","year":1953,"price":22.50,"authorId":1,"categoryId":1}'

curl -X DELETE http://localhost:5011/api/books/1
curl http://localhost:5011/api/books
```

## Что приложить в PDF
1. Цель и постановка задачи.
2. Скрин структуры проекта.
3. Скрины моделей и `AppDbContext`.
4. Скрин миграции `InitialCreate`.
5. Скрины Swagger с успешными CRUD-запросами.
6. Скрин результата `GET /api/books` до и после `DELETE`.
7. Выводы и сложности.

## Возможные сложности
1. Несовместимая версия EF Core с `net8.0` (нужно использовать ветку `8.x`).
2. Ошибки внешних ключей при создании книги (если `AuthorId` или `CategoryId` не существуют).
3. Конфликт уникальности категории (одинаковый `Name`).

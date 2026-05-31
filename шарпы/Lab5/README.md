# Лаба 5: Маршрутизация в ASP.NET Core

Проект реализует маршрутизацию на контроллерах с базовым маршрутом `api/[controller]` для ресурсов:

- `products`
- `students`
- `orders`

## Что реализовано

1. CRUD-маршруты для каждого контроллера:
   - `GET api/<controller>`
   - `GET api/<controller>/{id}`
   - `POST api/<controller>`
   - `PUT api/<controller>/{id}`
   - `DELETE api/<controller>/{id}`
2. Маршруты с параметрами: `{id}`, `{name}`, `{year}`
3. Ограничения маршрутов:
   - `{id:int}`
   - `{date:datetime}`
   - `{guid:guid}`
   - `{slug:minlength(3)}`
4. Необязательные параметры и значения по умолчанию:
   - `{id:int?}` (например, `api/products/details/{id?}`)
   - значение по умолчанию в маршруте `api/products/archive/{year:int=2026}`
5. Вложенный маршрут:
   - `GET api/students/{id}/courses`
6. Query-параметры для пагинации, фильтрации и сортировки:
   - `?page=1&pageSize=10&sort=name`
   - дополнительные фильтры: `year`, `status`

## Запуск

```bash
dotnet run --project Lab5/Lab5.csproj
```

Swagger UI:

- `https://localhost:<port>/swagger` или `http://localhost:<port>/swagger`

## Тестирование

Результаты проверок маршрутизации см. в файле:

- `Lab5/TEST_RESULTS.md`

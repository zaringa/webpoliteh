# Результаты тестирования маршрутизации (Lab5)

Дата проверки: 15 марта 2026

| URL | HTTP | Метод контроллера | Ожидаемый результат |
|---|---|---|---|
| `/api/products` | GET | `ProductsController.GetAll` | `200 OK`, список товаров с пагинацией и сортировкой |
| `/api/products/1` | GET | `ProductsController.GetById` | `200 OK`, товар с `id=1` |
| `/api/products` | POST | `ProductsController.Create` | `201 Created`, созданный товар + `Location` |
| `/api/products/4` | PUT | `ProductsController.Update` | `200 OK`, обновленный товар |
| `/api/products/4` | DELETE | `ProductsController.Delete` | `204 No Content` |
| `/api/products/by-name/Laptop` | GET | `ProductsController.GetByName` | `200 OK`, фильтрация по `{name}` |
| `/api/products/by-slug/keyboard-rgb` | GET | `ProductsController.GetBySlug` | `200 OK`, сработал маршрут `{slug:minlength(3)}` |
| `/api/products/by-slug/ab` | GET | Маршрут не найден | `404 Not Found`, ограничение `{slug:minlength(3)}` не прошло |
| `/api/products/by-date/2026-03-01T08:30:00` | GET | `ProductsController.GetByDate` | `200 OK`, сработал маршрут `{date:datetime}` |
| `/api/products/by-guid/8e700804-4adc-4ae4-b298-09d6397048db` | GET | `ProductsController.GetByGuid` | `200 OK`, сработал маршрут `{guid:guid}` |
| `/api/products/details` | GET | `ProductsController.GetDetails` | `200 OK`, вызов с необязательным `{id?}` без `id` |
| `/api/products/details/1` | GET | `ProductsController.GetDetails` | `200 OK`, вызов с `{id?}` и значением |
| `/api/products/archive` | GET | `ProductsController.GetByYear` | `200 OK`, использовано значение по умолчанию `year=2026` |
| `/api/students?page=1&pageSize=1&sort=name&year=2025` | GET | `StudentsController.GetAll` | `200 OK`, query-параметры пагинации/сортировки/фильтра |
| `/api/students/1` | GET | `StudentsController.GetById` | `200 OK`, студент с `id=1` |
| `/api/students/1/courses` | GET | `StudentsController.GetCoursesByStudentId` | `200 OK`, вложенный маршрут `students/{id}/courses` |
| `/api/students/by-name/Anna` | GET | `StudentsController.GetByName` | `200 OK`, фильтрация по `{name}` |
| `/api/students/by-year/2025` | GET | `StudentsController.GetByYear` | `200 OK`, фильтрация по `{year:int}` |
| `/api/orders?page=1&pageSize=1&sort=amount&status=paid` | GET | `OrdersController.GetAll` | `200 OK`, query-параметры пагинации/сортировки/фильтра |
| `/api/orders/2` | GET | `OrdersController.GetById` | `200 OK`, заказ с `id=2` |
| `/api/orders/by-date/2026-02-05T11:30:00` | GET | `OrdersController.GetByDate` | `200 OK`, сработал маршрут `{date:datetime}` |
| `/api/orders/tracking/4c875bc2-9d26-4a12-8bbf-c1bd094f8f19` | GET | `OrdersController.GetByTrackingId` | `200 OK`, сработал маршрут `{guid:guid}` |
| `/api/orders/tracking/not-a-guid` | GET | Маршрут не найден | `404 Not Found`, ограничение `{guid:guid}` не прошло |
| `/api/orders/summary` | GET | `OrdersController.GetSummary` | `200 OK`, необязательный `{id?}` без `id` |
| `/api/orders/summary/2` | GET | `OrdersController.GetSummary` | `200 OK`, необязательный `{id?}` с `id` |
| `/swagger/v1/swagger.json` | GET | Swagger endpoint | `200 OK`, маршруты корректно описаны в OpenAPI |

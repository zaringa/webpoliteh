# Результаты тестирования Lab4 (ASP.NET Core Web API)

Дата проверки: 15 марта 2026

| URL | HTTP | Какой метод вызывается | Ожидаемый результат |
|---|---|---|---|
| `/api/products` | GET | `ProductsController.GetAll` | `200 OK`, массив продуктов |
| `/api/products/1` | GET | `ProductsController.GetById` | `200 OK`, один продукт с `id=1` |
| `/api/products/999` | GET | `ProductsController.GetById` | `404 Not Found`, объект не найден |
| `/api/products` | POST | `ProductsController.Create` | `201 Created`, созданный продукт + заголовок `Location` |
| `/api/products` (невалидное тело) | POST | `ProductsController.Create` | `400 Bad Request`, ошибки валидации (`name`, `description`, `price`) |
| `/api/products/1` | PUT | `ProductsController.Update` | `204 No Content`, продукт обновлен |
| `/api/products/999` | PUT | `ProductsController.Update` | `404 Not Found`, объект не найден |
| `/api/products/2` | DELETE | `ProductsController.Delete` | `204 No Content`, продукт удален |
| `/api/products/999` | DELETE | `ProductsController.Delete` | `404 Not Found`, объект не найден |
| `/swagger/index.html` | GET | Swagger UI | `200 OK`, документация доступна в браузере |

# Lab13 - Подключение CSS и JavaScript в ASP.NET Core

## Цель
Ознакомиться с подключением и использованием файлов JavaScript и CSS в ASP.NET Core для клиентской части.

## Что сделано
1. Создана структура каталогов для клиентских ресурсов:
- `wwwroot/css`
- `wwwroot/js`
2. Добавлены несколько файлов:
- стили: `base.css`, `components.css`
- скрипты: `api.js`, `app.js`
3. Подключение в HTML:
- `<link rel="stylesheet" href="/css/base.css">`
- `<link rel="stylesheet" href="/css/components.css">`
- `<script type="module" src="/js/app.js"></script>`
4. Реализовано клиентское поведение:
- фильтрация данных по параметрам;
- сортировка по цене;
- кнопка проверки backend-ошибки;
- отображение статуса запросов.
5. Реализованы запросы с клиента на backend:
- `GET /api/catalog`
- `GET /api/catalog/summary`
- `GET /api/catalog/categories`
6. Реализована визуализация:
- таблица товаров;
- карточки сводки;
- столбчатая диаграмма по категориям.

## Ключевые файлы
- `Program.cs` - backend endpoint'ы + раздача статики.
- `wwwroot/index.html` - разметка страницы.
- `wwwroot/css/base.css` - базовые стили.
- `wwwroot/css/components.css` - стили компонентов.
- `wwwroot/js/api.js` - клиент API.
- `wwwroot/js/app.js` - логика UI и визуализация.

## Запуск
```bash
cd /home/zarina/Данные/webpoliteh/Lab13
dotnet restore
dotnet build
dotnet run --no-launch-profile --urls http://localhost:5013
```

Открыть:
- `http://localhost:5013`
- `http://localhost:5013/api/catalog`

## Сценарий показа на защите
1. Показать структуру папок `wwwroot/css` и `wwwroot/js`.
2. Показать `index.html` с тегами `link` и `script`.
3. Показать `Program.cs` (endpoint'ы и `UseStaticFiles`).
4. В браузере открыть главную страницу:
- изменить фильтр и нажать "Загрузить данные";
- нажать "Сортировка";
- показать обновление таблицы и графика.
5. Нажать "Проверка ошибки backend" и показать сообщение об ошибке.
6. Показать напрямую API-ответ в Swagger-like формате через браузер/`curl`.

## Что положить в PDF
1. Цель и задачи.
2. Скрины структуры каталогов `wwwroot`.
3. Скрины `index.html`, CSS и JS файлов.
4. Скрин работающей страницы (таблица + карточки + диаграмма).
5. Скрин запросов к backend (`/api/catalog`, `/api/catalog/summary`).
6. Выводы и возможные сложности.

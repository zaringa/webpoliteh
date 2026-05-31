# SportProg SPA

SPA-приложение по спортивному программированию: каталог алгоритмов, задач и учебных подборок в формате, похожем на медиакаталог, но вместо фильмов используются темы и задачи уровня Codeforces.

В качестве тематического ориентира использован список тем brestprog: https://brestprog.by/topics/

## Что реализовано

- React SPA на Vite с адаптивным интерфейсом.
- 9 маршрутов: главная, алгоритмы, детальная тема, задачи, подборки, детальная подборка, рейтинг, избранное, профиль, авторизация.
- Регистрация и авторизация пользователя.
- ASP.NET Core Web API.
- Собственная SQLite-база через Entity Framework Core.
- Сидирование базы алгоритмами, задачами, подборками, демо-пользователями, избранным и отправками.
- Взаимодействие frontend с backend через axios и Vite proxy.

## Демо-пользователь

Email: `student@sportprog.local`

Пароль: `demo123`

## Запуск

Backend:

```bash
cd backend
dotnet restore
dotnet run
```

API запускается на `http://localhost:5042`.

Frontend:

```bash
cd frontend
npm install
npm run dev
```

SPA открывается на `http://localhost:5173`.

## Проверка сборки

```bash
cd backend
dotnet build
```

```bash
cd frontend
npm run build
```

SQLite-файл `backend/sportprog.db` создается автоматически при первом запуске backend.

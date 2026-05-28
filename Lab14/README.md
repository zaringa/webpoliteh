# Lab14 - CORS в ASP.NET Core

## Цель
Ознакомиться с настройкой и использованием CORS для обработки кросс-доменных запросов.

## Что реализовано
1. Создан проект ASP.NET Core Web API (`Lab14`).
2. Добавлена поддержка CORS через `AddCors` + `UseCors`.
3. Реализованы 3 политики:
- `FrontendReadOnly`
  - Origins: `http://localhost:5500`, `http://127.0.0.1:5500`
  - Methods: `GET`
  - Headers: `Content-Type`, `X-Client-Version`
- `TrustedSpa`
  - Origins: `http://localhost:5173`, `http://127.0.0.1:5173`
  - Methods: `GET, POST, PUT, DELETE`
  - Headers: любые
  - Credentials: разрешены
- `AnyOriginGet`
  - Origin: любой
  - Methods: `GET`
  - Headers: любые
4. Настроены endpoint'ы:
- `/api/cors/no-policy` - без CORS-политики (для сравнения).
- `/api/cors/frontend-read` - политика `FrontendReadOnly`.
- `/api/cors/trusted` - политика `TrustedSpa`.
- `/api/cors/open` - политика `AnyOriginGet`.
5. Добавлен внешний клиент для теста из другого origin:
- `ExternalClient/index.html`

## Ключевые файлы
- `Program.cs` - конфигурация CORS и endpoint'ов.
- `appsettings.json` - `Cors:Enabled`, списки origins.
- `Lab14.http` - примеры запросов.
- `ExternalClient/index.html` - внешний клиент для браузерной проверки.

## Запуск API
```bash
cd /home/zarina/Данные/webpoliteh/Lab14
dotnet restore
dotnet build
dotnet run --no-launch-profile --urls http://localhost:5014
```

## Проверка CORS через curl
1. Разрешенный origin + policy:
```bash
curl -i http://localhost:5014/api/cors/frontend-read \
  -H "Origin: http://localhost:5500" \
  -H "X-Client-Version: 1.0"
```
Ожидается заголовок:
`Access-Control-Allow-Origin: http://localhost:5500`

2. Disallowed origin:
```bash
curl -i http://localhost:5014/api/cors/frontend-read \
  -H "Origin: http://evil.example.com"
```
Ожидается: нет `Access-Control-Allow-Origin`.

3. Endpoint без политики:
```bash
curl -i http://localhost:5014/api/cors/no-policy \
  -H "Origin: http://localhost:5500"
```
Ожидается: нет CORS-заголовков.

4. Preflight с запрещенным методом (`POST` для `FrontendReadOnly`):
```bash
curl -i -X OPTIONS http://localhost:5014/api/cors/frontend-read \
  -H "Origin: http://localhost:5500" \
  -H "Access-Control-Request-Method: POST"
```

## Проверка из внешнего клиента (браузер)
1. В отдельном терминале:
```bash
cd /home/zarina/Данные/webpoliteh/Lab14/ExternalClient
python3 -m http.server 5500
```
2. Открыть `http://localhost:5500`.
3. Нажать кнопки:
- `FrontendReadOnly` -> должно работать.
- `No policy` -> браузер блокирует CORS.
- `Trusted` -> браузер блокирует CORS для origin `:5500`.
- `AnyOriginGet` -> должно работать.

## Сравнение при включенном и выключенном CORS
По умолчанию CORS включен (`appsettings.json`: `Cors:Enabled = true`).

Запуск с отключенным CORS:
```bash
Cors__Enabled=false dotnet run --no-launch-profile --urls http://localhost:5014
```

Сравнение:
1. При `Cors:Enabled=true` endpoint'ы с политиками отдают `Access-Control-Allow-Origin` для разрешенных origins.
2. При `Cors:Enabled=false` применяются "deny-all" политики, поэтому CORS-заголовки отсутствуют у всех endpoint'ов.
3. В браузере cross-origin запросы становятся заблокированными даже к endpoint'ам, которые раньше были разрешены.

## Что показать на защите
1. Код политик CORS в `Program.cs`.
2. Настройки origins/methods/headers в `appsettings.json`.
3. Успешный и неуспешный запросы с разными `Origin`.
4. Preflight пример (`OPTIONS`).
5. Сравнение результата при `Cors:Enabled=true/false`.

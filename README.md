# Проект Веб-API на ASP.NET Core

RESTful веб-сервис для обмена сообщениями между пользователями. Реализован на ASP.NET Core в рамках дипломной работы по курсу C#-разработки.

## Технологии

- **.NET 8** (несколько приложений в одном решении)
- **Ocelot** (API Gateway), **MMLib.SwaggerForOcelot** (единый Swagger для Gateway)
- **PostgreSQL** (хранение пользователей и сообщений)
- **JWT, подпись RSA** (аутентификация)
- **Autofac** (DI в UserService)
- **AutoMapper** (DTO и Entity в MessageService)
- **xUnit** (тестирование)
- **Swagger UI** (проверка API на каждом сервисе и агрегация на Gateway)

## Структура решения

- `src/ApiGateway` — API Gateway (Ocelot): единая точка входа для UserService и MessageService, Swagger для обоих сервисов (порт 6000)
- `src/UserService` — регистрация, вход по email и паролю, выдача JWT (порт 5103)
- `src/MessageService` — отправка и получение сообщений (по UserID из JWT) (порт 5003)
- `src/OAuth` — вход через Google и обмен code на JWT (exchange-flow) (порт 5187)
- `tests/UnitTest` — модульные и интеграционные тесты
- `docs/` — PRD и планы

## Функционал

### Сервис пользователей (UserService)

- Регистрация и вход по **email + пароль**
- Хранение паролей: **SHA512 + salt** (RSA только для подписи JWT)
- Роли: Admin / User (не более одного Admin)
- JWT с claims: **UserID** (числовой), Role
- **Методы API:** добавить администратора (первый пользователь) — `POST /Login/AddAdmin`; добавить пользователя — `POST /Login/AddUser`; вход — `POST /Login`; получить список пользователей (только Admin) — `GET /api/Users`; удалить пользователя (только Admin, нельзя удалить себя) — `DELETE /api/Users/{id}`; метод, возвращающий ID по JWT — `GET /Restricted/Me` (ответ `{ "userId", "role" }`); для OAuth — `POST /api/Users/google-ensure`.

### Сервис сообщений (MessageService)

- Отправка сообщения: тело `{ "text": "...", "receiverId": <id> }`, отправитель из JWT
- Получение сообщений: для текущего пользователя (receiverId из JWT)
- При получении списка сообщения помечаются прочитанными
- Фильтрация по получателю (текущий пользователь)

### OAuth (Google)

- Вход через Google; после успешного входа приложение получает JWT из UserService
- **Exchange-flow**: клиент получает одноразовый `code` в редиректе, затем вызывает `POST /api/auth/exchange` с телом `{ "code": "..." }` и получает в ответе `{ "token": "..." }`

## Установка

1. Клонировать репозиторий:
   ```bash
   git clone https://github.com/AlekseyFomichenko/Diploma_Project_Web-API_ASP.NET_Core.git
   cd Diploma_Project_Web-API_ASP.NET_Core
   ```

2. Запустить PostgreSQL (например, в контейнере) и задать строки подключения в:
   - `src/UserService/appsettings.json` — `ConnectionStrings:DefaultConnection`
   - `src/MessageService/appsettings.json` — `ConnectionStrings:DefaultConnection`

3. Ключи RSA для JWT:
   - **UserService** (`src/UserService/rsa/`): `private_key.pem` (подпись JWT) и при необходимости `public_key.pem` (проверка своих API).
   - **MessageService**, **OAuth** и др.: только копия `public_key.pem` (проверка подписи). Приватный ключ — только в UserService. Подробнее: `docs/PRD.md`, раздел «Безопасность: JWT и ключи».

4. При необходимости применить миграции UserService:
   ```bash
   dotnet ef database update --project src/UserService
   ```

5. Запуск (в указанном порядке):
   - UserService: `dotnet run --project src/UserService` (http://localhost:5103)
   - MessageService: `dotnet run --project src/MessageService` (http://localhost:5003)
   - ApiGateway: `dotnet run --project src/ApiGateway` (http://localhost:6000)
   - OAuth (по необходимости): `dotnet run --project src/OAuth` (http://localhost:5187)  
   Клиенты обращаются к API пользователей и сообщений через **http://localhost:6000** (Gateway); Swagger доступен по http://localhost:6000/swagger. OAuth — по своему URL (прямой доступ). В `src/OAuth/appsettings.json` указать `UserService:BaseUrl` и `ClientRedirectUri` (куда редиректить с `?code=...` после Google).

## Тестирование

Запуск тестов:
```bash
dotnet test tests/UnitTest
```

Пример модульного теста (мок репозитория с SenderId/ReceiverId):
```cs
[Fact]
public void MockMessageRepo_SendMessage_AddsMessage()
{
    var repo = new MockMessageRepo();
    var result = repo.SendMessage("New text", 1, 2);
    Assert.Contains(repo.Messages, m => m.Text == "New text" && m.SenderId == 1 && m.ReceiverId == 2);
}
```

Пример вызова API через Gateway (http://localhost:6000; Login по email, MessageManager с JWT):
- Первый пользователь (админ): `POST http://localhost:6000/Login/AddAdmin` — body `{ "email": "...", "password": "..." }`
- Регистрация: `POST http://localhost:6000/Login/AddUser` — body `{ "email": "...", "password": "..." }`
- Вход: `POST http://localhost:6000/Login` — body `{ "email": "...", "password": "..." }` → в ответе `{ "token": "..." }`
- ID по JWT: `GET http://localhost:6000/Restricted/Me` с заголовком `Authorization: Bearer <token>` → `{ "userId": 1, "role": "Admin" }`
- Список пользователей (Admin): `GET http://localhost:6000/api/Users` с заголовком `Authorization: Bearer <token>`
- Удалить пользователя (Admin): `DELETE http://localhost:6000/api/Users/{id}` с заголовком `Authorization: Bearer <token>`
- Сообщения: `GET http://localhost:6000/api/MessageManager/GetMessages` и `POST http://localhost:6000/api/MessageManager/SendMessage` с заголовком `Authorization: Bearer <token>`, тело отправки: `{ "text": "...", "receiverId": 2 }`

Подробнее: [docs/PRD.md](docs/PRD.md).

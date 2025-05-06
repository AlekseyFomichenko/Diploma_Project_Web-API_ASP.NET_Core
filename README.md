# Проект Веб-АPI на ASP.NET Core

Проект представляет собой RESTful веб-сервис, реализованный на платформе ASP.NET Core. Сервис предназначен для обмена сообщениями между пользователями и включает в себя функционал отправки, получения и хранения сообщений. Проект реализован в рамках дипломной работы по курсу C#-разработки.

## 🔧 Технологии
- **ASP.NET Core 6** (микросервисы)
- **PostgreSQL** (хранение пользователей/сообщений)
- **JWT + RSA** (аутентификация)
- **Autofac** (DI)
- **AutoMapper** (DTO <-> Entity)
- **xUnit** (тестирование)
- **Swagger UI** (тестирование контроллеров)

## 🚀 Функционал
### **Сервис пользователей**
- Регистрация/аутентификация (email + пароль)
- RSA-шифрование паролей
- Ролевая модель (Admin/User)
- JWT с Claim (UserID)

### **Сервис сообщений**
- Отправка/получение сообщений
- Отметка о прочтении
- Фильтрация по получателю

## 🛠 Установка
1. Клонировать репозиторий:
   ```bash
   git clone https://github.com/AlekseyFomichenko/Diploma_Project_Web-API_ASP.NET_Core.git
## 📊 Тестирование

Пример теста:
```cs
[Fact]
        public void SendMessage_AddsNewMessageAndReturnsItsId()
        {
            // Arrange
            const string text = "Test Message";
            const string userFrom = "John";
            const string userTo = "Jane";

            // Act
            var result = _service.SendMessage(text, userFrom, userTo);

            // Assert
            Assert.Equal(3, result);
            Assert.Contains(_mockRepo._messages, m => m.Id == 3 && m.Text == text && m.UserFrom == userFrom && m.UserTo == userTo);
        }
```
```cs
[Theory]
        [InlineData("/api/values/1")]
        [InlineData("/api/values/2")]
        public async Task GetById_ReturnsOkResult(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
```

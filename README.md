# VTodoManager Backend
### О проекте
Backend проект таск-менеджера на ASP.NET Core 6.0. 

### Основной стек технологий
* ASP.NET Core 6.0
* Entity Framework Core
* PostgreSQL 14
* MediatR
* Swashbuckle (Swagger)
* AutoMapper
* Docker

### Архитектура проекта
За основу взята архитектура https://github.com/denis-tsv/CleanArchitecture/tree/master/Maximum
* Vtodo.Controllers - Слой, содержащий контроллеры проекта
* Vtodo.DataAccess.Postgres - Слой, содержащий реализацию доступа к базе данных Postgres
* Vtodo.DataAccess.Postgres.Migrations - Слой, содержащий миграции
* Vtodo.DomainServices.Implementation - Слой, содержащий реализацию сервисов бизнес-логики (без доступа к БД)
* Vtodo.DomainServices.Interfaces - Слой, содержащий интерфейсы сервисов бизнес-логики
* Vtodo.Entities - Слой, содержащий сущности проекта (модели, исключения, перечисления и т.д.)
* Vtodo.Infrastructure.Implementation - Слой, содержащий реализацию сервисов инфраструктуры
* Vtodo.Infrastructure.Implementation.Tests.Unit - Слой, содержащий юнит-тесты для Vtodo.Infrastructure.Implementation
* Vtodo.Infrastructure.Interfaces - Слой, содержащий интерфейсы сервисов инфраструктуры
* Vtodo.Tests - Слой тестов проекта (интеграционные и т.д. Также содержит вспомогательные классы для тестирования)
* Vtodo.UseCases - Слой, содержащий логику команд и запросов, DTO, AutoMapper профили
* Vtodo.UseCases.Tests.Unit - Слой, содержащий юнит-тесты для Vtodo.UseCases
* Vtodo.Web - Слой, содержащий веб-приложение

### Запуск проекта
* Склонировать репозиторий `git clone https://github.com/TVSergey/VTodo_Backend_AspNetCore.git`
* Перейти в корневой каталог репозитория
* Запустить проект, выполнив `docker-compose up --build`
* Перейти на `http://localhost:8080/`

Для доступа к основным функциям API необходимо создать аккаунт с помощью POST `/api/v1/accounts`, например:
    `{
        "email": "test@test.com",
        "username": "test",
        "password": "testpassw",
        "confirmPassword": "testpassw"
    }`
и затем указать accessToken в меню Authorize. 

По умолчанию accessToken действителен 5 минут. Это время можно изменить в файле конфигураций 
Vtodo.Web/appsettings.json. 

Также возможно получить новую пару accessToken\`a и refreshToken\`a используя POST `/api/v1/accounts/refresh_tokens`

### ToDo
* [ ] Добавить кеш (Redis)
* [ ] Добавить SignalR
* [ ] Добавить Email интеграцию
* [ ] Добавить настройки аккаунта и проектов
* [ ] Добавить логирование

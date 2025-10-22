**HabitTracker** — это веб-приложение для отслеживания привычек, разработанное как курсовой проект студента 2 курса. Система позволяет пользователям создавать привычки, отслеживать их выполнение и мотивировать себя через социальные функции.

## Основные возможности

- **Регистрация и авторизация** пользователей
- **Создание и управление** привычками
- **Отслеживание прогресса** с визуализацией
- **Социальные функции** (поиск друзей, уведомления)
- **Адаптивный дизайн** для всех устройств
- **Безопасное хранение** паролей

## Технологический стек

- **Backend:** ASP.NET Core 8.0, C#
- **Frontend:** HTML5, CSS3, JavaScript, Bootstrap
- **База данных:** SQLite
- **ORM:** Entity Framework Core
- **Аутентификация:** Cookie-based authentication
- **Инструменты:** Visual Studio Code, Git


## Структура проекта

```
HabitTracker/
├── Models/              # Модели данных
│   ├── User.cs
│   ├── Habit.cs
│   ├── HabitCompletion.cs
│   ├── Friend.cs
│   └── FriendRequest.cs
├── Data/               # Контекст базы данных
│   └── GutDbContext.cs
├── Pages/              # Razor Pages
│   ├── Index.cshtml    # Главная страница
│   ├── Login.cshtml    # Авторизация
│   ├── Register.cshtml # Регистрация
│   ├── Dashboard.cshtml # Панель управления
│   ├── MyHabits.cshtml # Управление привычками
│   ├── FindFriends.cshtml # Поиск друзей
│   └── Notifications.cshtml # Уведомления
├── wwwroot/            # Статические файлы
├── Migrations/         # Миграции БД
└── Program.cs          # Точка входа
```

## Makefile

```bash
make help      # Показать справку
make build     # Собрать проект
make run       # Запустить приложение
make stop      # Остановить приложение
make clean     # Очистить проект
make migrate   # Применить миграции
make reset-db  # Сбросить базу данных
make status    # Показать статус
make report    # Создать отчет проекта
```

## База данных

### Основные таблицы:
- **Users** — пользователи системы
- **Habits** — привычки пользователей
- **HabitCompletions** — отметки о выполнении
- **Friends** — связи дружбы
- **FriendRequests** — запросы в друзья

### Миграции:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

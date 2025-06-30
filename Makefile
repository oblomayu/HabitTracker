# Makefile for HabitTracker Project
# Курсовой проект "Система отслеживания привычек"

.PHONY: help build run clean migrate reset-db status stop report

# Default target
help:
	@echo "HabitTracker - Курсовой проект"
	@echo ""
	@echo "Доступные команды:"
	@echo "  build     - Собрать проект"
	@echo "  run       - Запустить приложение"
	@echo "  stop      - Остановить приложение"
	@echo "  clean     - Очистить проект"
	@echo "  migrate   - Применить миграции базы данных"
	@echo "  reset-db  - Сбросить базу данных"
	@echo "  status    - Показать статус приложения"
	@echo "  report    - Создать отчет проекта"
	@echo "  help      - Показать эту справку"

# Build the project
build:
	@echo "🔨 Сборка проекта..."
	dotnet build
	@echo "✅ Проект собран успешно!"

# Run the application
run: stop
	@echo "🚀 Запуск приложения..."
	@echo "📱 Приложение будет доступно по адресу: http://localhost:5216"
	@echo "⏹️  Для остановки нажмите Ctrl+C"
	dotnet run

# Stop running application
stop:
	@echo "🛑 Остановка приложения..."
	@pkill -f "dotnet.*HabitTracker" || true
	@echo "✅ Приложение остановлено"

# Clean the project
clean:
	@echo "🧹 Очистка проекта..."
	dotnet clean
	rm -rf bin/ obj/
	@echo "✅ Проект очищен!"

# Apply database migrations
migrate:
	@echo "🗄️  Применение миграций базы данных..."
	dotnet ef database update
	@echo "✅ Миграции применены!"

# Reset database
reset-db:
	@echo "🔄 Сброс базы данных..."
	rm -f HabitTracker.db
	dotnet ef database update
	@echo "✅ База данных сброшена!"

# Show application status
status:
	@echo "📊 Статус приложения:"
	@echo "  Проект: HabitTracker"
	@echo "  Порт: 5216"
	@echo "  База данных: SQLite"
	@if pgrep -f "dotnet.*HabitTracker" > /dev/null; then \
		echo "  Статус: ✅ Запущено"; \
		echo "  PID: $$(pgrep -f 'dotnet.*HabitTracker')"; \
	else \
		echo "  Статус: ❌ Остановлено"; \
	fi
	@if [ -f "HabitTracker.db" ]; then \
		echo "  База данных: ✅ Существует"; \
		echo "  Размер: $$(ls -lh HabitTracker.db | awk '{print $$5}')"; \
	else \
		echo "  База данных: ❌ Не найдена"; \
	fi

# Create project report
report:
	@echo "📝 Создание отчета проекта..."
	@echo "📄 Основной отчет: Отчет_Курсовой_Проект_HabitTracker.md"
	@echo "📄 Приложение А (Код): Приложение_А_Исходный_код.md"
	@echo "📄 Приложение В (Диаграммы): Приложение_В_Диаграммы.md"
	@echo ""
	@echo "📋 Структура отчета:"
	@echo "  1. Титульный лист"
	@echo "  2. Содержание"
	@echo "  3. Введение"
	@echo "  4. Техническое задание"
	@echo "  5. Анализ требований"
	@echo "  6. Проектирование системы"
	@echo "  7. Реализация"
	@echo "  8. Тестирование"
	@echo "  9. Заключение"
	@echo "  10. Список литературы"
	@echo ""
	@echo "✅ Отчет готов! Отредактируйте файлы под свои данные."

# Development workflow
dev: build migrate run

# Production build
prod: clean build migrate
	@echo "🚀 Продакшн сборка готова!"
	@echo "Для запуска используйте: dotnet run --environment Production"

# Quick start for new developers
setup: clean build migrate run 
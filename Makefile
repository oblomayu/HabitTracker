
.PHONY: help build run clean migrate reset-db status stop report

build:
	dotnet build

run: stop
	dotnet run

stop:
	@pkill -f "dotnet.*HabitTracker" || true

clean:
	dotnet clean
	rm -rf bin/ obj/
	
migrate:
	dotnet ef database update

reset-db:
	rm -f HabitTracker.db
	dotnet ef database update

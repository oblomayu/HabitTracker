using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gut.Data;
using gut.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class MyHabitsModel : PageModel
{
    private readonly GutDbContext _context;

    public MyHabitsModel(GutDbContext context)
    {
        _context = context;
    }

    public List<Habit> Habits { get; set; } = new();
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            Habits = await _context.Habits
                .Where(h => h.UserId == userId)
                .Include(h => h.Completions)
                .OrderByDescending(h => h.Id)
                .ToListAsync();

            return Page();
        }
        catch (Exception)
        {
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostCreateHabitAsync(string habitName, string? habitDescription)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return RedirectToPage("/Login");
        }

        if (string.IsNullOrWhiteSpace(habitName))
        {
            ErrorMessage = "Habit name is required.";
            return await OnGetAsync();
        }

        try
        {
            var habit = new Habit
            {
                Name = habitName.Trim(),
                Description = habitDescription?.Trim(),
                UserId = userId
            };

            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();

            Message = "Habit created successfully!";
            return RedirectToPage();
        }
        catch (Exception)
        {
            ErrorMessage = "Failed to create habit. Please try again.";
            return await OnGetAsync();
        }
    }

    public async Task<IActionResult> OnPostToggleCompletionAsync(int habitId, string date)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            // Проверяем, что привычка принадлежит пользователю
            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

            if (habit == null)
            {
                return RedirectToPage();
            }

            if (DateTime.TryParse(date, out DateTime targetDate))
            {
                var completion = await _context.HabitCompletions
                    .FirstOrDefaultAsync(c => c.HabitId == habitId && c.Date.Date == targetDate.Date);

                if (completion != null)
                {
                    // Переключаем статус
                    completion.Completed = !completion.Completed;
                }
                else
                {
                    // Создаем новую запись
                    completion = new HabitCompletion
                    {
                        HabitId = habitId,
                        Date = targetDate.Date,
                        Completed = true
                    };
                    _context.HabitCompletions.Add(completion);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
        catch (Exception)
        {
            return RedirectToPage();
        }
    }

    public async Task<IActionResult> OnPostDeleteHabitAsync(int habitId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

            if (habit != null)
            {
                _context.Habits.Remove(habit);
                await _context.SaveChangesAsync();
                Message = "Habit deleted successfully!";
            }

            return RedirectToPage();
        }
        catch (Exception)
        {
            ErrorMessage = "Failed to delete habit. Please try again.";
            return await OnGetAsync();
        }
    }

    public bool IsCompletedToday(int habitId)
    {
        var today = DateTime.Today;
        return Habits
            .FirstOrDefault(h => h.Id == habitId)?
            .Completions
            .Any(c => c.Date.Date == today && c.Completed) ?? false;
    }
} 
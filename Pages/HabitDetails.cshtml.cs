using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gut.Data;
using gut.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class HabitDetailsModel : PageModel
{
    private readonly GutDbContext _context;

    public HabitDetailsModel(GutDbContext context)
    {
        _context = context;
    }

    public Habit? Habit { get; set; }
    public int TotalCompletions { get; set; }
    public double SuccessRate { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public bool IsCompletedToday { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            Habit = await _context.Habits
                .Include(h => h.Completions.OrderByDescending(c => c.Date))
                .FirstOrDefaultAsync(h => h.Id == id && h.UserId == userId);

            if (Habit == null)
            {
                return Page();
            }

            CalculateStats();
            return Page();
        }
        catch (Exception)
        {
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostToggleTodayAsync(int habitId)
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

            if (habit == null)
            {
                return RedirectToPage("/MyHabits");
            }

            var today = DateTime.Today;
            var completion = await _context.HabitCompletions
                .FirstOrDefaultAsync(c => c.HabitId == habitId && c.Date.Date == today);

            if (completion != null)
            {
                completion.Completed = !completion.Completed;
            }
            else
            {
                completion = new HabitCompletion
                {
                    HabitId = habitId,
                    Date = today,
                    Completed = true
                };
                _context.HabitCompletions.Add(completion);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = habitId });
        }
        catch (Exception)
        {
            return RedirectToPage("/Error");
        }
    }

    private void CalculateStats()
    {
        if (Habit == null) return;

        var completions = Habit.Completions.Where(c => c.Completed).ToList();
        TotalCompletions = completions.Count;
        
        var totalDays = Habit.Completions.Count;
        SuccessRate = totalDays > 0 ? (TotalCompletions * 100.0 / totalDays) : 0;

        // Вычисляем текущую серию
        CurrentStreak = CalculateCurrentStreak();
        
        // Вычисляем самую длинную серию
        LongestStreak = CalculateLongestStreak();

        // Проверяем, выполнена ли привычка сегодня
        IsCompletedToday = Habit.Completions.Any(c => c.Date.Date == DateTime.Today && c.Completed);
    }

    private int CalculateCurrentStreak()
    {
        if (Habit == null) return 0;

        var today = DateTime.Today;
        var streak = 0;
        var currentDate = today;

        while (true)
        {
            var completion = Habit.Completions.FirstOrDefault(c => c.Date.Date == currentDate);
            if (completion != null && completion.Completed)
            {
                streak++;
                currentDate = currentDate.AddDays(-1);
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    private int CalculateLongestStreak()
    {
        if (Habit == null) return 0;

        var completions = Habit.Completions.Where(c => c.Completed).OrderBy(c => c.Date).ToList();
        if (!completions.Any()) return 0;

        var longestStreak = 0;
        var currentStreak = 1;
        var previousDate = completions[0].Date;

        for (int i = 1; i < completions.Count; i++)
        {
            var currentDate = completions[i].Date;
            if (currentDate == previousDate.AddDays(1))
            {
                currentStreak++;
            }
            else
            {
                longestStreak = Math.Max(longestStreak, currentStreak);
                currentStreak = 1;
            }
            previousDate = currentDate;
        }

        longestStreak = Math.Max(longestStreak, currentStreak);
        return longestStreak;
    }
} 
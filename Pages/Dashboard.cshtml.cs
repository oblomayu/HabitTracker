using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gut.Data;
using gut.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class DashboardModel : PageModel
{
    private readonly GutDbContext _context;

    public DashboardModel(GutDbContext context)
    {
        _context = context;
    }

    public User? CurrentUser { get; set; }
    public List<Habit> Habits { get; set; } = new();
    public DateTime CurrentDate { get; set; } = DateTime.Now;

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return RedirectToPage("/Login");
        }

        try
        {
            CurrentUser = await _context.Users
                .Include(u => u.Habits)
                .ThenInclude(h => h.Completions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (CurrentUser == null)
            {
                return RedirectToPage("/Login");
            }

            Habits = CurrentUser.Habits;

            return Page();
        }
        catch (Exception ex)
        {
            // Log the exception in a real application
            return RedirectToPage("/Error");
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
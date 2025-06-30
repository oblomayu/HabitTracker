using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gut.Data;
using gut.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class NotificationsModel : PageModel
{
    private readonly GutDbContext _context;

    public NotificationsModel(GutDbContext context)
    {
        _context = context;
    }

    public List<FriendRequestWithUser> FriendRequests { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return RedirectToPage("/Login");

        // Получаем запросы в друзья для текущего пользователя
        var requests = await _context.FriendRequests
            .Include(fr => fr.FromUser)
            .Where(fr => fr.ToUserId == userId && !fr.IsAccepted && !fr.IsDeclined)
            .OrderByDescending(fr => fr.CreatedAt)
            .ToListAsync();

        FriendRequests = requests.Select(fr => new FriendRequestWithUser
        {
            Id = fr.Id,
            FromUser = fr.FromUser,
            CreatedAt = fr.CreatedAt
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAcceptRequestAsync(int requestId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return RedirectToPage("/Login");

        try
        {
            var request = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.Id == requestId && fr.ToUserId == userId);

            if (request != null)
            {
                // Принимаем запрос
                request.IsAccepted = true;
                request.RespondedAt = DateTime.UtcNow;

                // Добавляем дружбу
                _context.Friends.Add(new Friend
                {
                    UserId = request.FromUserId,
                    FriendUserId = request.ToUserId
                });

                // Добавляем обратную дружбу
                _context.Friends.Add(new Friend
                {
                    UserId = request.ToUserId,
                    FriendUserId = request.FromUserId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
        catch (Exception)
        {
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostDeclineRequestAsync(int requestId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return RedirectToPage("/Login");

        try
        {
            var request = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.Id == requestId && fr.ToUserId == userId);

            if (request != null)
            {
                request.IsDeclined = true;
                request.RespondedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
        catch (Exception)
        {
            return RedirectToPage("/Error");
        }
    }
}

public class FriendRequestWithUser
{
    public int Id { get; set; }
    public User FromUser { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
} 
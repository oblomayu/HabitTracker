using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gut.Data;
using gut.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class FindFriendsModel : PageModel
{
    private readonly GutDbContext _context;
    public FindFriendsModel(GutDbContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }
    public List<User> SearchResults { get; set; } = new();
    public List<User> Friends { get; set; } = new();
    public List<int> FriendIds { get; set; } = new();
    public List<int> PendingRequests { get; set; } = new();
    public int CurrentUserId { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return RedirectToPage("/Login");
        CurrentUserId = userId;

        // Поиск пользователей
        if (!string.IsNullOrWhiteSpace(Query))
        {
            SearchResults = await _context.Users
                .Where(u => u.Username.Contains(Query) && u.Id != userId)
                .ToListAsync();
        }

        // Список друзей
        FriendIds = await _context.Friends
            .Where(f => f.UserId == userId)
            .Select(f => f.FriendUserId)
            .ToListAsync();
        Friends = await _context.Users
            .Where(u => FriendIds.Contains(u.Id))
            .ToListAsync();

        // Список отправленных запросов
        PendingRequests = await _context.FriendRequests
            .Where(fr => fr.FromUserId == userId && !fr.IsAccepted && !fr.IsDeclined)
            .Select(fr => fr.ToUserId)
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAddFriendAsync(int FriendUserId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            return RedirectToPage("/Login");
        CurrentUserId = userId;

        // Проверяем, не отправлен ли уже запрос
        bool alreadyRequested = await _context.FriendRequests.AnyAsync(fr => 
            fr.FromUserId == userId && fr.ToUserId == FriendUserId && !fr.IsAccepted && !fr.IsDeclined);
        
        // Проверяем, не друзья ли уже
        bool alreadyFriend = await _context.Friends.AnyAsync(f => 
            f.UserId == userId && f.FriendUserId == FriendUserId);
        
        if (!alreadyRequested && !alreadyFriend && FriendUserId != userId)
        {
            _context.FriendRequests.Add(new FriendRequest 
            { 
                FromUserId = userId, 
                ToUserId = FriendUserId 
            });
            await _context.SaveChangesAsync();
        }
        return RedirectToPage(new { query = Query });
    }
} 
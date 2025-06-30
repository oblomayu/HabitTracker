using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gut.Data;
using gut.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

public class RegisterModel : PageModel
{
    private readonly GutDbContext _context;

    public RegisterModel(GutDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string RepeatPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Password != RepeatPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters long.";
            return Page();
        }

        try
        {
            // Check if username already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);
            if (existingUser != null)
            {
                ErrorMessage = "Username already exists. Please choose a different one.";
                return Page();
            }

            var hashedPassword = HashPassword(Password);
            var user = new User { Username = Username, Password = hashedPassword };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Login");
        }
        catch (Exception ex)
        {
            ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
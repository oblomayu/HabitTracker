using System.ComponentModel.DataAnnotations;

namespace gut.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
    
    public List<Habit> Habits { get; set; } = new();
}
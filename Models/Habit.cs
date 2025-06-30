using System.ComponentModel.DataAnnotations;

namespace gut.Models;

public class Habit
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<HabitCompletion> Completions { get; set; } = new();
}
using System.ComponentModel.DataAnnotations;

namespace gut.Models;

public class HabitCompletion
{
    public int Id { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public bool Completed { get; set; }
    
    public int HabitId { get; set; }
    public Habit Habit { get; set; } = null!;
}
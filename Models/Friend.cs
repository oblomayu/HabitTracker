using System.ComponentModel.DataAnnotations;

public class Friend
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; } // Текущий пользователь

    [Required]
    public int FriendUserId { get; set; } // Друг

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
} 
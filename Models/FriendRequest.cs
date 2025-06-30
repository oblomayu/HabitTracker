using System.ComponentModel.DataAnnotations;
using gut.Models;

public class FriendRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int FromUserId { get; set; } // Кто отправляет запрос

    [Required]
    public int ToUserId { get; set; } // Кому отправляется запрос

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsAccepted { get; set; } = false;
    public bool IsDeclined { get; set; } = false;
    public DateTime? RespondedAt { get; set; }

    // Навигационные свойства
    public User FromUser { get; set; } = null!;
    public User ToUser { get; set; } = null!;
} 
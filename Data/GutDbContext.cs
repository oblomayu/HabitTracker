using Microsoft.EntityFrameworkCore;
using gut.Models;

namespace gut.Data;

public class GutDbContext : DbContext
{
    public GutDbContext(DbContextOptions<GutDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitCompletion> HabitCompletions { get; set; }
    public DbSet<Friend> Friends { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }
}
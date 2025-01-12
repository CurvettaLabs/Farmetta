using Microsoft.EntityFrameworkCore;
using WebPush;

namespace NotificationManager.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }
    
    public DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }
    public DbSet<Vapid> VapidDetails { get; set; }
}
using Microsoft.EntityFrameworkCore;

namespace CommandCenter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        public DbSet<TwitchChatMessage> TwitchChatMessages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TwitchChatMessage>()
                .HasIndex(m => m.UserId);
        }
    }
}
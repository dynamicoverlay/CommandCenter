﻿using Microsoft.EntityFrameworkCore;

namespace CommandCenter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        public DbSet<TwitchChatMessage> TwitchChatMessages { get; set; }
        public DbSet<Channel> Channels { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.HasPostgresExtension("uuid-ossp");
            builder.Entity<Channel>(eb =>
            {
                eb.Property(ch => ch.Id)
                    .HasDefaultValueSql("uuid_generate_v4()");
            });

            builder.Entity<TwitchChatMessage>()
                .HasIndex(m => m.UserId);
        }
    }
}
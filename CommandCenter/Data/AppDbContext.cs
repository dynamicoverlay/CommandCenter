using Microsoft.EntityFrameworkCore;

namespace CommandCenter.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        
        public DbSet<Fruit> Fruits { get; set; }
        
        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);
            
            model.HasPostgresExtension("uuid-ossp");
            model.Entity<Fruit>(eb =>
            {
                eb.Property(fr => fr.Id)
                    .HasDefaultValueSql("uuid_generate_v4()");
            });
        }
    }
}
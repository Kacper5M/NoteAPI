using Microsoft.EntityFrameworkCore;

namespace NoteAPI.Entities
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            { 
                builder.Property(r => r.FirstName).IsRequired().HasMaxLength(15);
                builder.Property(r => r.LastName).IsRequired().HasMaxLength(15);
                builder.Property(r => r.Email).IsRequired();
                builder.Property(r => r.PasswordHash).IsRequired();
            });

            modelBuilder.Entity<Role>(builder =>
            {
                builder.Property(r => r.Name).IsRequired();
            });

            modelBuilder.Entity<Note>(builder =>
            {
                builder.Property(r => r.NoteTitle).HasMaxLength(50);
                builder.Property(r => r.NoteBody).IsRequired().HasMaxLength(500);
                builder.Property(r => r.UserId).IsRequired();
            });
        }
    }
}

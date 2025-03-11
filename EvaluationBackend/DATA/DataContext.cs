using EvaluationBackend.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using IXIR.Entities;

namespace EvaluationBackend.DATA
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Store>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Store>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "DataEntry" }
            );

            base.OnModelCreating(modelBuilder);
        }

        public static void SeedAdminUser(DataContext context, string adminUsername, string adminPassword)
        {
            // Ensure that the roles exist in the DB
            var adminRole = context.Roles.SingleOrDefault(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role { Name = "Admin" };
                context.Roles.Add(adminRole);
                context.SaveChanges();
            }

            // Check if admin user already exists
            var existingAdmin = context.Users.SingleOrDefault(u => u.UserName == adminUsername);
            if (existingAdmin != null) return;  // Skip if already seeded

            // Create admin user
            var adminUser = new AppUser
            {
                UserName = adminUsername,
                FullName = "Admin",
                Password = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                RoleId = adminRole.Id
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
        }
    }
}

using EvaluationBackend.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using IXIR.Entities;
using System.Linq;

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

            // Many-to-Many Relationship Between Store and Product
            modelBuilder.Entity<Store>()
                .HasMany(s => s.Products)
                .WithMany(p => p.Stores)
                .UsingEntity(j => j.ToTable("StoreProducts")); 

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "DataEntry" }
            );

            base.OnModelCreating(modelBuilder);
        }

        public static void SeedAdminUser(DataContext context, string adminUsername, string adminPassword)
        {
            var adminRole = context.Roles.SingleOrDefault(r => r.Name == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role { Name = "Admin" };
                context.Roles.Add(adminRole);
                context.SaveChanges();
            }

            // Check if admin user already exists
            var existingAdmin = context.Users.SingleOrDefault(u => u.UserName == adminUsername);
            if (existingAdmin != null) return;  

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

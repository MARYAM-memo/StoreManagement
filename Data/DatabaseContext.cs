using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Areas.Users.Models;
using StoreManagement.Models;
using StoreManagement.Models.Inventory;

namespace StoreManagement.Data;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>(options)
{
      public DbSet<Category> Categories { get; set; }
      public DbSet<Brand> Brands { get; set; }
      public DbSet<Product> Products { get; set; }
      public DbSet<Customer> Customers { get; set; }
      public DbSet<Order> Orders { get; set; }
      public DbSet<OrderItem> OrderItems { get; set; }
      public DbSet<Supplier> Suppliers { get; set; }
      public DbSet<StockTransaction> StockTransactions { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            base.OnModelCreating(modelBuilder);
            var fixedDate = new DateTime(2026, 01, 06, 9, 47, 00, DateTimeKind.Utc);

            // Customize ASP.NET Identity model
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                  entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                  entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                  entity.Property(e => e.ProfilePicture).HasMaxLength(200);
                  entity.Property(e => e.EmployeeId).HasMaxLength(20);
                  entity.Property(e => e.Department).HasMaxLength(100);
                  entity.Property(e => e.Position).HasMaxLength(100);
                  entity.Property(e => e.CreatedAt).IsRequired();
                  entity.Property(e => e.IsActive).HasDefaultValue(true);
                  entity.Property(e => e.ForcePasswordChange).HasDefaultValue(false);
            });

            modelBuilder.Entity<ApplicationRole>(entity =>
            {
                  entity.Property(e => e.Description).HasMaxLength(500);
                  entity.Property(e => e.CreatedAt).IsRequired();
                  entity.Property(e => e.CreatedBy).HasMaxLength(100);
                  entity.Property(e => e.IsSystemRole).HasDefaultValue(false);

                  // Set default values for permission properties
                  entity.Property(e => e.CanManageUsers).HasDefaultValue(false);
                  entity.Property(e => e.CanManageRoles).HasDefaultValue(false);
                  entity.Property(e => e.CanManageProducts).HasDefaultValue(false);
                  entity.Property(e => e.CanManageCategories).HasDefaultValue(false);
                  entity.Property(e => e.CanManageBrands).HasDefaultValue(false);
                  entity.Property(e => e.CanManageCustomers).HasDefaultValue(false);
                  entity.Property(e => e.CanManageOrders).HasDefaultValue(false);
                  entity.Property(e => e.CanManageSuppliers).HasDefaultValue(false);
                  entity.Property(e => e.CanManageInventory).HasDefaultValue(false);
                  entity.Property(e => e.CanViewReports).HasDefaultValue(false);
                  entity.Property(e => e.CanManageSettings).HasDefaultValue(false);
            });

            // Configure UserRole entity if using custom UserRole class
            modelBuilder.Entity<UserRole>(entity =>
            {
                  entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                  entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .IsRequired();

                  entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .IsRequired();

                  entity.Property(ur => ur.AssignedDate).IsRequired();
                  entity.Property(ur => ur.AssignedBy).HasMaxLength(100);
            });

            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Order)
                .WithMany(i => i.OrderItems)
                .HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId);
            modelBuilder.Entity<Category>().HasData(
                 new Category
                 {
                       Id = 1,
                       Name = "Electronics",
                       Description = "Electronic devices and accessories",
                       IsActive = true,
                       CreatedAt = fixedDate,
                 },
                 new Category
                 {
                       Id = 2,
                       Name = "Fashion",
                       Description = "Clothing and fashion products",
                       IsActive = true,
                       CreatedAt = fixedDate,
                 },
                 new Category
                 {
                       Id = 3,
                       Name = "Home",
                       Description = "Home and Kitchen",
                       IsActive = true,
                       CreatedAt = fixedDate,
                 },
                 new Category
                 {
                       Id = 4,
                       Name = "Toys",
                       Description = "Toys and Games",
                       IsActive = true,
                       CreatedAt = fixedDate,
                 }
             );
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Apple" },
                new Brand { Id = 2, Name = "Nike" },
                new Brand { Id = 3, Name = "SamSung" },
                new Brand { Id = 4, Name = "Sony" },
                new Brand { Id = 5, Name = "Honor" },
                new Brand { Id = 6, Name = "Hp" },
                new Brand { Id = 7, Name = "Lenovo" }
            );
            modelBuilder.Entity<Product>().HasData(
                  new Product
                  {
                        Id = 1,
                        Name = "iPhone 15",
                        Description = "Latest Apple smartphone",
                        Price = 45000,
                        StockQuantity = 20,
                        CategoryId = 1,
                        BrandId = 1,
                        IsActive = true,
                        CreatedAt = fixedDate,
                  },
                  new Product
                  {
                        Id = 2,
                        Name = "Nike Air Max",
                        Description = "Running shoes",
                        Price = 3500,
                        StockQuantity = 50,
                        CategoryId = 2,
                        BrandId = 2,
                        IsActive = true,
                        CreatedAt = fixedDate,
                  }
            );

            // Configure table names (اختياري)
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            //modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
      }

}

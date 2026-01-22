// في Data/SeedData.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Areas.Users.Models;

namespace StoreManagement.Data
{
      public static class SeedData
      {
            public static async Task Initialize(IServiceProvider serviceProvider)
            {
                  using (var scope = serviceProvider.CreateScope())
                  {
                        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                        // Create database if not exists
                        await context.Database.MigrateAsync();

                        // Seed Roles
                        await SeedRolesAsync(roleManager);

                        // Seed Admin User
                        await SeedAdminUserAsync(userManager, roleManager);
                  }
            }

            private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
            {
                  string[] roleNames = ["SuperAdmin", "Admin", "Manager", "Sales", "Viewer", "User"];

                  foreach (var roleName in roleNames)
                  {
                        var roleExist = await roleManager.RoleExistsAsync(roleName);
                        if (!roleExist)
                        {
                              var role = new ApplicationRole
                              {
                                    Name = roleName,
                                    NormalizedName = roleName.ToUpper(),
                                    CreatedAt = DateTime.UtcNow,
                                    CreatedBy = "System",
                                    IsSystemRole = roleName is "SuperAdmin" or "Admin" or "User",
                                    Description = GetRoleDescription(roleName)
                              };

                              // Set permissions based on role
                              SetRolePermissions(role, roleName);

                              await roleManager.CreateAsync(role);
                        }
                  }
            }

            private static string GetRoleDescription(string roleName)
            {
                  return roleName switch
                  {
                        "SuperAdmin" => "Full system administrator with all permissions",
                        "Admin" => "Administrator with management permissions",
                        "Manager" => "Store manager with operational permissions",
                        "Sales" => "Sales staff with customer and order permissions",
                        "Viewer" => "View-only role with minimal permissions",
                        "User" => "Basic user role for new registrations",
                        _ => "User role"
                  };
            }

            private static void SetRolePermissions(ApplicationRole role, string roleName)
            {
                  switch (roleName)
                  {
                        case "SuperAdmin":
                              role.CanManageUsers = true;
                              role.CanManageRoles = true;
                              role.CanManageProducts = true;
                              role.CanManageCategories = true;
                              role.CanManageBrands = true;
                              role.CanManageCustomers = true;
                              role.CanManageOrders = true;
                              role.CanManageSuppliers = true;
                              role.CanManageInventory = true;
                              role.CanViewReports = true;
                              role.CanManageSettings = true;
                              break;

                        case "Admin":
                              role.CanManageUsers = true;
                              role.CanManageRoles = true;
                              role.CanManageProducts = true;
                              role.CanManageCategories = true;
                              role.CanManageBrands = true;
                              role.CanManageCustomers = true;
                              role.CanManageOrders = true;
                              role.CanManageSuppliers = true;
                              role.CanManageInventory = true;
                              role.CanViewReports = true;
                              role.CanManageSettings = false;
                              break;

                        case "Manager":
                              role.CanManageUsers = false;
                              role.CanManageRoles = false;
                              role.CanManageProducts = true;
                              role.CanManageCategories = true;
                              role.CanManageBrands = true;
                              role.CanManageCustomers = true;
                              role.CanManageOrders = true;
                              role.CanManageSuppliers = true;
                              role.CanManageInventory = true;
                              role.CanViewReports = true;
                              role.CanManageSettings = false;
                              break;

                        case "Sales":
                              role.CanManageUsers = false;
                              role.CanManageRoles = false;
                              role.CanManageProducts = false;
                              role.CanManageCategories = false;
                              role.CanManageBrands = false;
                              role.CanManageCustomers = true;
                              role.CanManageOrders = true;
                              role.CanManageSuppliers = false;
                              role.CanManageInventory = false;
                              role.CanViewReports = true;
                              role.CanManageSettings = false;
                              break;

                        case "Viewer":
                              role.CanManageUsers = false;
                              role.CanManageRoles = false;
                              role.CanManageProducts = false;
                              role.CanManageCategories = false;
                              role.CanManageBrands = false;
                              role.CanManageCustomers = false;
                              role.CanManageOrders = false;
                              role.CanManageSuppliers = false;
                              role.CanManageInventory = false;
                              role.CanViewReports = true;
                              role.CanManageSettings = false;
                              break;

                        case "User":
                        default:
                              role.CanManageUsers = false;
                              role.CanManageRoles = false;
                              role.CanManageProducts = false;
                              role.CanManageCategories = false;
                              role.CanManageBrands = false;
                              role.CanManageCustomers = false;
                              role.CanManageOrders = false;
                              role.CanManageSuppliers = false;
                              role.CanManageInventory = false;
                              role.CanViewReports = false;
                              role.CanManageSettings = false;
                              break;
                  }
            }

            private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
            {
                  string adminEmail = "admin@store.com";
                  string adminPassword = "Admin@123";

                  var adminUser = await userManager.FindByEmailAsync(adminEmail);

                  if (adminUser == null)
                  {
                        adminUser = new ApplicationUser
                        {
                              UserName = adminEmail,
                              Email = adminEmail,
                              FirstName = "System",
                              LastName = "Administrator",
                              EmailConfirmed = true,
                              PhoneNumber = "+1234567890",
                              EmployeeId = "SYS001",
                              Department = "IT",
                              Position = "System Administrator",
                              IsActive = true,
                              CreatedAt = DateTime.UtcNow,
                              HireDate = DateTime.UtcNow,
                              ForcePasswordChange = false
                        };

                        var result = await userManager.CreateAsync(adminUser, adminPassword);

                        if (result.Succeeded)
                        {
                              // Add to SuperAdmin and Admin roles
                              if (await roleManager.RoleExistsAsync("SuperAdmin"))
                                    await userManager.AddToRoleAsync(adminUser, "SuperAdmin");

                              if (await roleManager.RoleExistsAsync("Admin"))
                                    await userManager.AddToRoleAsync(adminUser, "Admin");

                              adminUser.LastPasswordChangeDate = DateTime.UtcNow;
                              await userManager.UpdateAsync(adminUser);
                        }
                  }
            }
      }
      /*       public static class SeedData
            {
                  public static async Task Initialize(DatabaseContext context,
                      UserManager<ApplicationUser> userManager,
                      RoleManager<ApplicationRole> roleManager)
                  {
                        // Ensure database is created
                        await context.Database.EnsureCreatedAsync();

                        // Seed roles
                        if (!await roleManager.Roles.AnyAsync())
                        {
                              await SeedRolesAsync(roleManager);
                        }

                        // Seed default admin user
                        if (!await userManager.Users.AnyAsync())
                        {
                              await SeedAdminUserAsync(userManager, roleManager);
                        }
                  }

                  private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
                  {
                        // SuperAdmin Role - Full permissions
                        var superAdminRole = new ApplicationRole
                        {
                              Name = "SuperAdmin",
                              Description = "System administrator with full permissions",
                              IsSystemRole = true,
                              CreatedAt = DateTime.UtcNow,
                              CreatedBy = "System",
                              CanManageUsers = true,
                              CanManageRoles = true,
                              CanManageProducts = true,
                              CanManageCategories = true,
                              CanManageBrands = true,
                              CanManageCustomers = true,
                              CanManageOrders = true,
                              CanManageSuppliers = true,
                              CanManageInventory = true,
                              CanViewReports = true,
                              CanManageSettings = true
                        };

                        // Admin Role - Most permissions
                        var adminRole = new ApplicationRole
                        {
                              Name = "Admin",
                              Description = "Administrator with management permissions",
                              IsSystemRole = true,
                              CreatedAt = DateTime.UtcNow,
                              CreatedBy = "System",
                              CanManageUsers = true,
                              CanManageRoles = true,
                              CanManageProducts = true,
                              CanManageCategories = true,
                              CanManageBrands = true,
                              CanManageCustomers = true,
                              CanManageOrders = true,
                              CanManageSuppliers = true,
                              CanManageInventory = true,
                              CanViewReports = true,
                              CanManageSettings = false
                        };

                        // Manager Role - Limited management permissions
                        var managerRole = new ApplicationRole
                        {
                              Name = "Manager",
                              Description = "Store manager with operational permissions",
                              IsSystemRole = false,
                              CreatedAt = DateTime.UtcNow,
                              CreatedBy = "System",
                              CanManageUsers = false,
                              CanManageRoles = false,
                              CanManageProducts = true,
                              CanManageCategories = true,
                              CanManageBrands = true,
                              CanManageCustomers = true,
                              CanManageOrders = true,
                              CanManageSuppliers = true,
                              CanManageInventory = true,
                              CanViewReports = true,
                              CanManageSettings = false
                        };

                        // Sales Role - Basic permissions
                        var salesRole = new ApplicationRole
                        {
                              Name = "Sales",
                              Description = "Sales staff with basic permissions",
                              IsSystemRole = false,
                              CreatedAt = DateTime.UtcNow,
                              CreatedBy = "System",
                              CanManageUsers = false,
                              CanManageRoles = false,
                              CanManageProducts = false,
                              CanManageCategories = false,
                              CanManageBrands = false,
                              CanManageCustomers = true,
                              CanManageOrders = true,
                              CanManageSuppliers = false,
                              CanManageInventory = false,
                              CanViewReports = true,
                              CanManageSettings = false
                        };

                        // Viewer Role - Read-only permissions
                        var viewerRole = new ApplicationRole
                        {
                              Name = "Viewer",
                              Description = "View-only role with minimal permissions",
                              IsSystemRole = false,
                              CreatedAt = DateTime.UtcNow,
                              CreatedBy = "System",
                              CanManageUsers = false,
                              CanManageRoles = false,
                              CanManageProducts = false,
                              CanManageCategories = false,
                              CanManageBrands = false,
                              CanManageCustomers = false,
                              CanManageOrders = false,
                              CanManageSuppliers = false,
                              CanManageInventory = false,
                              CanViewReports = true,
                              CanManageSettings = false
                        };

                        // Create roles
                        var roles = new[] { superAdminRole, adminRole, managerRole, salesRole, viewerRole };

                        foreach (var role in roles)
                        {
                              if (!await roleManager.RoleExistsAsync(role.Name ?? ""))
                              {
                                    await roleManager.CreateAsync(role);
                              }
                        }
                  }

                  private static async Task SeedAdminUserAsync(
                      UserManager<ApplicationUser> userManager,
                      RoleManager<ApplicationRole> roleManager)
                  {
                        // Create default admin user
                        var adminUser = new ApplicationUser
                        {
                              UserName = "admin@store.com",
                              Email = "admin@store.com",
                              FirstName = "System",
                              LastName = "Administrator",
                              PhoneNumber = "+1234567890",
                              EmployeeId = "SYS001",
                              Department = "IT",
                              Position = "System Administrator",
                              IsActive = true,
                              CreatedAt = DateTime.UtcNow,
                              HireDate = DateTime.UtcNow,
                              ForcePasswordChange = false
                        };

                        // Create user
                        var result = await userManager.CreateAsync(adminUser, "Admin@123");

                        if (result.Succeeded)
                        {
                              // Add to SuperAdmin role
                              if (await roleManager.RoleExistsAsync("SuperAdmin"))
                              {
                                    await userManager.AddToRoleAsync(adminUser, "SuperAdmin");
                              }

                              // Add to Admin role as well
                              if (await roleManager.RoleExistsAsync("Admin"))
                              {
                                    await userManager.AddToRoleAsync(adminUser, "Admin");
                              }

                              // Update last password change date
                              adminUser.LastPasswordChangeDate = DateTime.UtcNow;
                              await userManager.UpdateAsync(adminUser);
                        }
                  }
            } */
}
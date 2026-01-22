using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Areas.Users.Models;
using StoreManagement.Data;
using StoreManagement.DataAccess;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.DataAccess.Services;

var builder = WebApplication.CreateBuilder(args);

var storeConnect = builder.Configuration.GetConnectionString("storeDbConnection");
builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseNpgsql(storeConnect));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
// Configure Identity with custom user and role types
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";

    // SignIn settings
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<DatabaseContext>()
.AddDefaultTokenProviders()
.AddUserManager<UserManager<ApplicationUser>>()
.AddRoleManager<RoleManager<ApplicationRole>>()
.AddSignInManager<SignInManager<ApplicationUser>>();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.LoginPath = "/Users/Account/Login";
    options.LogoutPath = "/Users/Account/Logout";
    options.AccessDeniedPath = "/Users/Account/AccessDenied";
    options.SlidingExpiration = true;
});


// Add authorization policies
builder.Services.AddAuthorizationBuilder()
                                   // Add authorization policies
                                   .AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin", "SuperAdmin"))
                                   // Add authorization policies
                                   .AddPolicy("RequireSuperAdminRole", policy =>
        policy.RequireRole("SuperAdmin"))
                                   // Add authorization policies
                                   .AddPolicy("CanManageUsers", policy =>
        policy.RequireClaim("Permission", "CanManageUsers"))
                                   // Add authorization policies
                                   .AddPolicy("CanManageRoles", policy =>
        policy.RequireClaim("Permission", "CanManageRoles"));

// Add controllers with views
builder.Services.AddControllersWithViews();

// Add session support (مفيد لإدارة الجلسات)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add logging
builder.Services.AddLogging();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Seed default users and roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.MapAreaControllerRoute(
    name: "UsersArea",
    areaName: "Users",
    pattern: "Users/{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

/*
StoreDashboard
│
├── Areas
│   ├── Inventory
│   │   ├── Controllers
│   │   │   ├── SuppliersController.cs
│   │   │   └── StockTransactionsController.cs
│   │   ├── Views
│   │   │   ├── Suppliers
│   │   │   └── StockTransaction
│   ├── Sales
│   │   ├── Controllers
│   │   │   ├── CustomersController.cs
│   │   │   ├── OrdersController.cs
│   │   │   └── OrderItemsController.cs
│   │   ├── Views
│   │   │   ├── Customers
│   │   │   ├── Orders
│   │   │   └── OrderItems
│   ├── Users
│   │   ├── Controllers
│   │   │   ├── UsersController.cs
│   │   │   ├── RolesController.cs
│   │   │   └── UserRolesController.cs
│   │   ├── Views
│   │   │   ├── Users
│   │   │   ├── Roles
│   │   │   └── UserRoles
├── Controllers
│   ├── ProductsController.cs
│   ├── CategoriesController.cs
│   └── BrandsController.cs
│   └── HomeController.cs
├── ViewModels
│   ├── ProductVM.cs
│   ├── OrderVM.cs
│   └── DashboardStatsVM.cs
├── Models
│   ├── Brand.cs
│   ├── Category.cs
│   ├── Customer.cs
│   ├── Order.cs
│   ├── OrderItem.cs
│   └── Product.cs
├── DataAccess
│   ├── Base
│   │   ├── IRepository.cs 
│   │   └── IUnitOfWork.cs 
│   ├── Repository.cs
│   └── UnitOfWork.cs
├── Data
│   ├── Migrations
│   └── DatabaseContext.cs
├── Views
│   ├── Products
│   ├── Brands
│   └── Categories
*/
using System;
using Microsoft.AspNetCore.Identity;

namespace StoreManagement.Areas.Users.Models;

public class ApplicationRole : IdentityRole
{
      public string? Description { get; set; }

      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      public string? CreatedBy { get; set; }

      public bool IsSystemRole { get; set; } = false;

      // Permission Categories
      public bool CanManageUsers { get; set; }
      public bool CanManageRoles { get; set; }
      public bool CanManageProducts { get; set; }
      public bool CanManageCategories { get; set; }
      public bool CanManageBrands { get; set; }
      public bool CanManageCustomers { get; set; }
      public bool CanManageOrders { get; set; }
      public bool CanManageSuppliers { get; set; }
      public bool CanManageInventory { get; set; }
      public bool CanViewReports { get; set; }
      public bool CanManageSettings { get; set; }

      // Navigation Properties
      public virtual IEnumerable<UserRole>? UserRoles { get; set; }
}

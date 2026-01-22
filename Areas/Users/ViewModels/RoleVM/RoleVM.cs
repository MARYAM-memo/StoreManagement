using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Areas.Users.ViewModels.RoleVM;

public class RoleVM
{
      public string? Id { get; set; }

      [Required(ErrorMessage = "Role name is required")]
      [Display(Name = "Role Name")]
      [StringLength(100)]
      public required string Name { get; set; }

      [Display(Name = "Description")]
      [StringLength(500)]
      public string? Description { get; set; }

      [Display(Name = "System Role")]
      public bool IsSystemRole { get; set; }

      public int UserCount { get; set; }
      public DateTime CreatedAt { get; set; }

      // Permissions
      [Display(Name = "Can Manage Users")]
      public bool CanManageUsers { get; set; }

      [Display(Name = "Can Manage Roles")]
      public bool CanManageRoles { get; set; }

      [Display(Name = "Can Manage Products")]
      public bool CanManageProducts { get; set; }

      [Display(Name = "Can Manage Categories")]
      public bool CanManageCategories { get; set; }

      [Display(Name = "Can Manage Brands")]
      public bool CanManageBrands { get; set; }

      [Display(Name = "Can Manage Customers")]
      public bool CanManageCustomers { get; set; }

      [Display(Name = "Can Manage Orders")]
      public bool CanManageOrders { get; set; }

      [Display(Name = "Can Manage Suppliers")]
      public bool CanManageSuppliers { get; set; }

      [Display(Name = "Can Manage Inventory")]
      public bool CanManageInventory { get; set; }

      [Display(Name = "Can View Reports")]
      public bool CanViewReports { get; set; }

      [Display(Name = "Can Manage Settings")]
      public bool CanManageSettings { get; set; }
}

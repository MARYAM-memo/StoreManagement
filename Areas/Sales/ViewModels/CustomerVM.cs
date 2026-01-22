using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Areas.Sales.ViewModels;

public class CustomerVM
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Customer name is required")]
      [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
      [Display(Name = "Full Name")]
      public required string Name { get; set; }

      [Phone(ErrorMessage = "Invalid phone number format")]
      [Display(Name = "Phone Number")]
      public string? Phone { get; set; }

      [EmailAddress(ErrorMessage = "Invalid email format")]
      public string? Email { get; set; }

      [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters")]
      public string? Address { get; set; }

      [StringLength(50)]
      public string? City { get; set; }

      [StringLength(50)]
      public string Country { get; set; } = "Egypt"; // Default

      [StringLength(20)]
      [Display(Name = "Postal Code")]
      public string? PostalCode { get; set; }

      [Display(Name = "Active")]
      public bool IsActive { get; set; } = true;

      [DataType(DataType.MultilineText)]
      [StringLength(1000)]
      public string? Notes { get; set; }

      // Read-only properties for display
      public int TotalOrders { get; set; }
      public decimal TotalSpent { get; set; }
      public DateTime? LastPurchaseDate { get; set; }
}

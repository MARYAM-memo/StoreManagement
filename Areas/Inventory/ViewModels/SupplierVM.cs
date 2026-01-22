using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Areas.Inventory.ViewModels;

public class SupplierVM
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Supplier name is required")]
      [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
      [Display(Name = "Supplier Name")]
      public string? Name { get; set; }

      [StringLength(100, ErrorMessage = "Contact person name cannot exceed 100 characters")]
      [Display(Name = "Contact Person")]
      public string? ContactPerson { get; set; }

      [Phone(ErrorMessage = "Invalid phone number format")]
      [StringLength(20)]
      public string? Phone { get; set; }

      [EmailAddress(ErrorMessage = "Invalid email format")]
      [StringLength(100)]
      public string? Email { get; set; }

      [Url(ErrorMessage = "Invalid website URL")]
      [StringLength(200)]
      public string? Website { get; set; }

      [StringLength(500)]
      public string? Address { get; set; }

      [StringLength(100)]
      public string? City { get; set; }

      [StringLength(100)]
      public string Country { get; set; } = "Egypt";

      [StringLength(20)]
      [Display(Name = "Postal Code")]
      public string? PostalCode { get; set; }

      [StringLength(50)]
      [Display(Name = "Tax Number")]
      public string? TaxNumber { get; set; }

      [StringLength(50)]
      [Display(Name = "Payment Terms")]
      public string? PaymentTerms { get; set; }

      [Display(Name = "Credit Limit")]
      [Range(0, double.MaxValue)]
      public decimal CreditLimit { get; set; }

      [Display(Name = "Current Balance")]
      [Range(0, double.MaxValue)]
      public decimal Balance { get; set; }

      [Display(Name = "Active")]
      public bool IsActive { get; set; } = true;

      [StringLength(1000)]
      [DataType(DataType.MultilineText)]
      public string? Notes { get; set; }

      // Statistics
      public int ProductCount { get; set; }
      public int TransactionCount { get; set; }
      public decimal TotalPurchases { get; set; }
}

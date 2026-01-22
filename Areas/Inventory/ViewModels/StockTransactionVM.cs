using System;
using System.ComponentModel.DataAnnotations;
using StoreManagement.Models;
using StoreManagement.Models.Inventory;

namespace StoreManagement.Areas.Inventory.ViewModels;

public class StockTransactionVM
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Product is required")]
      [Display(Name = "Product")]
      public int ProductId { get; set; }

      [Display(Name = "Supplier")]
      public int? SupplierId { get; set; }

      [Required(ErrorMessage = "Transaction type is required")]
      [Display(Name = "Transaction Type")]
      public string? TransactionType { get; set; }

      [Required(ErrorMessage = "Quantity is required")]
      [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
      public int Quantity { get; set; } = 1;

      [Required(ErrorMessage = "Unit cost is required")]
      [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0")]
      [Display(Name = "Unit Cost")]
      public decimal UnitCost { get; set; }

      [Required(ErrorMessage = "Transaction date is required")]
      [DataType(DataType.DateTime)]
      [Display(Name = "Transaction Date")]
      public DateTime TransactionDate { get; set; } = DateTime.Now;

      [StringLength(100)]
      [Display(Name = "Reference Number")]
      public string? ReferenceNumber { get; set; }

      [StringLength(100)]
      [Display(Name = "Reference Type")]
      public string? ReferenceType { get; set; }

      [Display(Name = "Reference ID")]
      public int? ReferenceId { get; set; }

      [StringLength(500)]
      [DataType(DataType.MultilineText)]
      public string? Notes { get; set; }

      // For display only
      public string? ProductName { get; set; }
      public string? SupplierName { get; set; }
      
      public int CurrentStock { get; set; }
      public decimal TotalCost => Quantity * UnitCost;

      // For dropdowns
      public List<Product> Products { get; set; } = [];
      public List<Supplier> Suppliers { get; set; } = [];
}

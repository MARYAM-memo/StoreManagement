using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Areas.Sales.ViewModels;

public class OrderItemVM
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Product is required")]
      [Display(Name = "Product")]
      public int ProductId { get; set; }

      [Required(ErrorMessage = "Quantity is required")]
      [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
      public int Quantity { get; set; } = 1;

      [Required(ErrorMessage = "Unit price is required")]
      [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
      [Display(Name = "Unit Price")]
      public decimal UnitPrice { get; set; }

      [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
      public decimal Discount { get; set; }

      // Calculated properties
      public decimal TotalPrice => Quantity * UnitPrice * (1 - Discount / 100);

      // For display
      public string? ProductName { get; set; }
      public int AvailableStock { get; set; }
}

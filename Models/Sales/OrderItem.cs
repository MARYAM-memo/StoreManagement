using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models;

public class OrderItem
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [Required]
      public int OrderId { get; set; }

      [Required]
      public int ProductId { get; set; }

      [Range(1, int.MaxValue)]
      public int Quantity { get; set; }

      [Column(TypeName = "decimal(18,2)")]
      public decimal UnitPrice { get; set; }
      public decimal TotalPrice => Quantity * UnitPrice;

      [MaxLength(100)]
      public string? ProductName { get; set; } // لحفظ اسم المنتج وقت الطلب

      public decimal? Discount { get; set; }

      // Navigation
      public Order? Order { get; set; }
      public Product? Product { get; set; }
}

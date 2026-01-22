using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models.Inventory;

public class StockTransaction
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [Required]
      public int ProductId { get; set; }

      public int? SupplierId { get; set; }

      [Required]
      [MaxLength(50)]
      public required string TransactionType { get; set; } // Purchase, Sale, Return, Adjustment, Transfer

      [Required]
      public int Quantity { get; set; }

      [Column(TypeName = "decimal(18,2)")]
      public decimal UnitCost { get; set; }

      [Column(TypeName = "decimal(18,2)")]
      public decimal TotalCost => Quantity * UnitCost;

      [Required]
      public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

      // Reference Info
      [MaxLength(100)]
      public string? ReferenceNumber { get; set; } // فاتورة الشراء، رقم الطلب، إلخ

      [MaxLength(100)]
      public string? ReferenceType { get; set; } // PurchaseOrder, SalesOrder, Adjustment

      public int? ReferenceId { get; set; } // ID من الجدول المرجعي

      [MaxLength(500)]
      public string? Notes { get; set; }

      public int? CreatedBy { get; set; } // User ID who created the transaction

      // Navigation Properties
      [ForeignKey("ProductId")]
      public virtual Product? Product { get; set; }

      [ForeignKey("SupplierId")]
      public virtual Supplier? Supplier { get; set; }
}

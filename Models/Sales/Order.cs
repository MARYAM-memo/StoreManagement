using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models;

public class Order
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [Required]
      public int CustomerId { get; set; }

      // Order Number (للتتبع)
      [MaxLength(50)]
      public string OrderNumber { get; set; } = GenerateOrderNumber();

      private DateTime _orderDate;
      public DateTime OrderDate
      {
            get => _orderDate;
            set => _orderDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
      }


      [Range(0, double.MaxValue)]
      public decimal TotalAmount { get; set; }

      [Required]
      [MaxLength(50)]
      public required string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled
      public DateTime? ShippedDate { get; set; }

      public DateTime? DeliveredDate { get; set; }

      [MaxLength(500)]
      public string? ShippingAddress { get; set; }

      [MaxLength(500)]
      public string? BillingAddress { get; set; }

      [MaxLength(50)]
      public string? PaymentMethod { get; set; } // Cash, Credit Card, PayPal, etc.

      [MaxLength(50)]
      public string? PaymentStatus { get; set; } // Paid, Pending, Failed

      public decimal ShippingCost { get; set; }

      public decimal TaxAmount { get; set; }

      public decimal DiscountAmount { get; set; }

      [MaxLength(1000)]
      public string? Notes { get; set; }

      // Navigation
      public Customer? Customer { get; set; }
      public ICollection<OrderItem>? OrderItems { get; set; }

      private static string GenerateOrderNumber()
      {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
      }
}

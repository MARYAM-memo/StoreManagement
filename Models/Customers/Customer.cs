using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models;

public class Customer
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [Required]
      [MaxLength(150)]
      public required string Name { get; set; }

      [Phone]
      public string? Phone { get; set; }

      [EmailAddress]
      public string? Email { get; set; }

      [MaxLength(300)]
      public string? Address { get; set; }

      // إضافات مفيدة
      [MaxLength(50)]
      public string? City { get; set; }

      [MaxLength(50)]
      public string? Country { get; set; }

      [MaxLength(20)]
      public string? PostalCode { get; set; }

      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      public DateTime? LastPurchaseDate { get; set; }

      public decimal TotalSpent { get; set; }

      public int TotalOrders { get; set; }

      public bool IsActive { get; set; } = true;

      public string? Notes { get; set; }

      // Navigation

      public ICollection<Order>? Orders { get; set; }

}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models.Inventory;

public class Supplier
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [Required]
      [MaxLength(150)]
      public required string Name { get; set; }

      [MaxLength(100)]
      public string? ContactPerson { get; set; }

      [Phone]
      [MaxLength(20)]
      public string? Phone { get; set; }

      [EmailAddress]
      [MaxLength(100)]
      public string? Email { get; set; }

      [MaxLength(200)]
      public string? Website { get; set; }

      [MaxLength(500)]
      public string? Address { get; set; }

      [MaxLength(100)]
      public string? City { get; set; }

      [MaxLength(100)]
      public string? Country { get; set; }

      [MaxLength(20)]
      public string? PostalCode { get; set; }

      // إضافات مفيدة
      [MaxLength(50)]
      public string? TaxNumber { get; set; } // الرقم الضريبي

      [MaxLength(50)]
      public string? PaymentTerms { get; set; } // شروط الدفع

      [Column(TypeName = "decimal(18,2)")]
      public decimal CreditLimit { get; set; }

      [Column(TypeName = "decimal(18,2)")]
      public decimal Balance { get; set; }

      public bool IsActive { get; set; } = true;

      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      public DateTime? UpdatedAt { get; set; }

      [MaxLength(1000)]
      public string? Notes { get; set; }

      // Navigation Properties
      public ICollection<StockTransaction>? StockTransactions { get; set; }
      public ICollection<Product>? Products { get; set; }
}

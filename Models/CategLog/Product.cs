using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StoreManagement.Models.Inventory;

namespace StoreManagement.Models;

public class Product
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [Required]
      [MaxLength(150)]
      public required string Name { get; set; }

      [MaxLength(1000)]
      public string? Description { get; set; }

      [Range(0, double.MaxValue)]
      [Column(TypeName = "decimal(18,2)")]
      public decimal Price { get; set; }

      [Range(0, int.MaxValue)]
      public int StockQuantity { get; set; }

      public string? ImageUrl { get; set; }

      public bool IsActive { get; set; } = true;

      private DateTime _createdAt;
      public DateTime CreatedAt
      {
            get => _createdAt;
            set => _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
      }
      private DateTime _updatedAt;
      public DateTime UpdatedAt
      {
            get => _updatedAt;
            set => _updatedAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
      }
      // Foreign Keys
      [Required]
      public int CategoryId { get; set; }

      [Required]
      public int BrandId { get; set; }

      // Navigation Properties
      public Category? Category { get; set; }
      public Brand? Brand { get; set; }

      public int? SupplierId { get; set; }

      [ForeignKey("SupplierId")]
      public virtual Supplier? Supplier { get; set; }
}

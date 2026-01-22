using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models;

public class Category
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [Required]
      [MaxLength(100)]
      public required string Name { get; set; }

      [MaxLength(500)]
      public string? Description { get; set; }

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

      // Navigation Property
      public ICollection<Product>? Products { get; set; }

      public override string ToString()
      {
            return @$"
            Id: {Id}
            Name: {Name},
            IsActive: {IsActive},
            Products: {Products?.Count ?? 0}";
      }
}

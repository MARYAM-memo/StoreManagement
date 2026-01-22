using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreManagement.Models;

public class Brand
{
      [Key]
      [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
      public int Id { get; set; }

      [Required]
      [MaxLength(100)]
      public required string Name { get; set; }

      public ICollection<Product>? Products { get; set; }
}

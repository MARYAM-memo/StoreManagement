using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.ViewModels;

public class BrandVM
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Brand name is required")]
      [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
      [Display(Name = "Brand Name")]
      public string? Name { get; set; }

      // Statistics (للعرض فقط)
      public int ProductCount { get; set; }
}


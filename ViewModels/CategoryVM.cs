using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.ViewModels;

public class CategoryVM
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Category name is required")]
      [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
      [Display(Name = "Category Name")]
      public string? Name { get; set; }

      [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
      [DataType(DataType.MultilineText)]
      public string? Description { get; set; }

      [Display(Name = "Active")]
      public bool IsActive { get; set; } = true;

      public DateTime CreatedAt{get; set;}
      public DateTime UpdatedAt{get; set;}

      // Statistics (للعرض فقط)
      public int ProductCount { get; set; }

}

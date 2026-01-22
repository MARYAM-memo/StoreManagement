using System;
using System.ComponentModel.DataAnnotations;
using StoreManagement.Models;

namespace StoreManagement.ViewModels;

public class ProductVM
{
      public int Id { get; set; }

      [Required(ErrorMessage = "Product name is required")]
      [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
      [Display(Name = "Product Name")]
      public string? Name { get; set; }

      [DataType(DataType.MultilineText)]
      [MaxLength(1000)]
      public string? Description { get; set; }

      [Required(ErrorMessage = "Price is required")]
      [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000")]
      [DataType(DataType.Currency)]
      public decimal Price { get; set; }

      [Required(ErrorMessage = "Quantity is required")]
      [Range(0, 1000000, ErrorMessage = "Quantity must be between 0 and 1,000,000")]
      [Display(Name = "Quantity in Stock")]
      public int StockQuantity { get; set; }

      [Display(Name = "Category")]
      [Required(ErrorMessage = "Category is required")]
      public int CategoryId { get; set; }

      [Display(Name = "Brand")]
      [Required(ErrorMessage = "Brand is required")]
      public int BrandId { get; set; }

      [Display(Name = "Active")]
      public bool IsActive { get; set; } = true;

      [Display(Name = "Image URL")]
      public string? ImageUrl { get; set; }

      // For dropdown lists
      public IEnumerable<Category>? Categories { get; set; }
      public IEnumerable<Brand>? Brands { get; set; }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Areas.Users.ViewModels.AccountVM;

public class ResetAccountPasswordVM
{
      [Required(ErrorMessage = "Email is required")]
      [EmailAddress(ErrorMessage = "Invalid email address")]
      [Display(Name = "Email")]
      public string? Email { get; set; }

      [Required(ErrorMessage = "Password is required")]
      [DataType(DataType.Password)]
      [Display(Name = "Password")]
      [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
      public string? Password { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "Confirm Password")]
      [Compare("Password", ErrorMessage = "Passwords do not match")]
      public string? ConfirmPassword { get; set; }

      public string? Token { get; set; }
}

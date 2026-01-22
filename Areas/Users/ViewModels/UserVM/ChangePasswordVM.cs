using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Areas.Users.ViewModels.UserVM;

public class ChangePasswordVM
{
      public string? UserId { get; set; }

      [Required(ErrorMessage = "Current password is required")]
      [DataType(DataType.Password)]
      [Display(Name = "Current Password")]
      public string? CurrentPassword { get; set; }

      [Required(ErrorMessage = "New password is required")]
      [DataType(DataType.Password)]
      [Display(Name = "New Password")]
      [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
      public string? NewPassword { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "Confirm New Password")]
      [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
      public string? ConfirmPassword { get; set; }
}

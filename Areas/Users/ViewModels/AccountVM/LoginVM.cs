using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Areas.Users.ViewModels.AccountVM;

public class LoginVM
{
      [Required(ErrorMessage = "Username or email is required")]
      [Display(Name = "Username or Email")]
      public string? UserName { get; set; }

      [Required(ErrorMessage = "Password is required")]
      [DataType(DataType.Password)]
      [Display(Name = "Password")]
      public string? Password { get; set; }

      [Display(Name = "Remember me")]
      public bool RememberMe { get; set; }
}

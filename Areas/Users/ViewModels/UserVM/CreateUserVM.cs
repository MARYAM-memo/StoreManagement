using System;
using System.ComponentModel.DataAnnotations;
using StoreManagement.Areas.Users.Models;

namespace StoreManagement.Areas.Users.ViewModels.UserVM;

public class CreateUserVM
{
      [Required(ErrorMessage = "First name is required")]
      [Display(Name = "First Name")]
      [StringLength(100)]
      public string? FirstName { get; set; }

      [Required(ErrorMessage = "Last name is required")]
      [Display(Name = "Last Name")]
      [StringLength(100)]
      public string? LastName { get; set; }

      [Required(ErrorMessage = "Email is required")]
      [EmailAddress(ErrorMessage = "Invalid email address")]
      [Display(Name = "Email")]
      public string? Email { get; set; }

      [Required(ErrorMessage = "Username is required")]
      [Display(Name = "Username")]
      [StringLength(100)]
      public string? UserName { get; set; }

      [Required(ErrorMessage = "Password is required")]
      [DataType(DataType.Password)]
      [Display(Name = "Password")]
      [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
      public string? Password { get; set; }

      [DataType(DataType.Password)]
      [Display(Name = "Confirm Password")]
      [Compare("Password", ErrorMessage = "Passwords do not match")]
      public string? ConfirmPassword { get; set; }

      [Phone(ErrorMessage = "Invalid phone number")]
      [Display(Name = "Phone Number")]
      public string? PhoneNumber { get; set; }

      [Display(Name = "Employee ID")]
      [StringLength(20)]
      public string? EmployeeId { get; set; }

      [Display(Name = "Department")]
      [StringLength(100)]
      public string? Department { get; set; }

      [Display(Name = "Position")]
      [StringLength(100)]
      public string? Position { get; set; }

      [Display(Name = "Profile Picture URL")]
      [Url(ErrorMessage = "Invalid URL")]
      public string? ProfilePicture { get; set; }

      [Display(Name = "Roles")]
      public List<string> SelectedRoles { get; set; } = [];

      // Available roles for dropdown
      public List<ApplicationRole> AvailableRoles { get; set; } = [];
}

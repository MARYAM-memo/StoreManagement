using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Areas.Users.ViewModels.AccountVM;

public class ProfileVM
{
      public string? Id { get; set; }

      [Required(ErrorMessage = "First name is required")]
      [Display(Name = "First Name")]
      [StringLength(100)]
      public string FirstName { get; set; } = "";

      [Required(ErrorMessage = "Last name is required")]
      [Display(Name = "Last Name")]
      [StringLength(100)]
      public string LastName { get; set; } = "";

      [Required(ErrorMessage = "Email is required")]
      [EmailAddress(ErrorMessage = "Invalid email address")]
      [Display(Name = "Email")]
      public string Email { get; set; } = "";

      [Required(ErrorMessage = "Username is required")]
      [Display(Name = "Username")]
      [StringLength(100)]
      public string UserName { get; set; } = "";

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

      // Read-only properties
      public string FullName => $"{FirstName} {LastName}";
      public DateTime? LastLoginDate { get; set; }
      public DateTime CreatedAt { get; set; }
}

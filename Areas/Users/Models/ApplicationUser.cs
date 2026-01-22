using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StoreManagement.Areas.Users.Models;

public class ApplicationUser : IdentityUser
{
      [Required]
      [MaxLength(100)]
      public required string FirstName { get; set; }

      [Required]
      [MaxLength(100)]
      public required string LastName { get; set; }

      [MaxLength(200)]
      public string? ProfilePicture { get; set; }

      // Additional Info
      [MaxLength(20)]
      public string? EmployeeId { get; set; }

      [MaxLength(100)]
      public string? Department { get; set; }

      [MaxLength(100)]
      public string? Position { get; set; }

      // Dates
      public DateTime? HireDate { get; set; }

      public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

      public DateTime? LastLoginDate { get; set; }

      public DateTime? LastPasswordChangeDate { get; set; }

      // Status
      public bool IsActive { get; set; } = true;

      public bool ForcePasswordChange { get; set; } = false;

      // Navigation Properties
      public virtual IEnumerable<UserRole>? UserRoles { get; set; }

      // Calculated Properties
      public string FullName => $"{FirstName} {LastName}";

      public override string ToString()
      {
            string user = @$"
            FirstName: {FirstName},
            LastName: {LastName},
            FullName: {FullName},
            ProfilePicture: {ProfilePicture},
            EmployeeId: {EmployeeId},
            Department: {Department},
            Position: {Position},
            HireDate: {HireDate},
            CreatedAt: {CreatedAt},
            LastLoginDate: {LastLoginDate},
            LastPasswordChangeDate: {LastPasswordChangeDate},
            IsActive: {IsActive},
            ForcePasswordChange: {ForcePasswordChange}
            ";
            if(UserRoles!=null)
            foreach(var role in UserRoles)
            {
                  user +=$"--- {role}";
            }
            return user;
      }
}

using System;
using Microsoft.AspNetCore.Identity;

namespace StoreManagement.Areas.Users.Models;

public class UserRole : IdentityUserRole<string>
{
      // يمكن إضافة خصائص إضافية إذا لزم الأمر
      public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

      public string? AssignedBy { get; set; }

      // Navigation Properties
      public virtual ApplicationUser? User { get; set; }
      public virtual ApplicationRole? Role { get; set; }

      public override string ToString()
      {
            string role = @$"
            UserRole: {User?.FullName} [{Role?.Description}]";
            return role;
      }
}

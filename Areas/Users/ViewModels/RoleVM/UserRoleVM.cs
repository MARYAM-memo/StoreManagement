using System;

namespace StoreManagement.Areas.Users.ViewModels.RoleVM;

public class UserRoleVM
{
      public string? UserId { get; set; }
      public string? UserName { get; set; }
      public string? FullName { get; set; }
      public string? Email { get; set; }
      public List<string> CurrentRoles { get; set; } = [];
      public List<string> AvailableRoles { get; set; } = [];
      public List<string> SelectedRoles { get; set; } = [];
}

using System;

namespace StoreManagement.Common
{
      public static class AppRoles
      {
            public const string Admin = "Admin";
            public const string Manager = "Manager";
            public const string Staff = "Staff";
            public const string Customer = "Customer";
      }

      public static class DefaultValues
      {
            public const string DefaultProductImage = "/css/images/products/default.jpg";
            public const int MaxUploadFileSizeMB = 5;
            public const string AdminEmail = "admin@storedashboard.com";
            public const string AdminPassword = "Admin@Store123";
            public const int DefaultStockQuantity = 0;
      }
}


using System;
using StoreManagement.Models;

namespace StoreManagement.ViewModels;

public class DashboardStatsVM
{
      public int TotalProducts { get; set; }
      public int TotalCustomers { get; set; }
      public int TotalOrders { get; set; }
      public decimal TotalRevenue { get; set; }
      public int LowStockProducts { get; set; }
      public int PendingOrders { get; set; }

      public List<Product> LowStockProductsList { get; set; }
      public List<Order> RecentOrders { get; set; }

      public DashboardStatsVM()
      {
            LowStockProductsList = [];
            RecentOrders = [];
      }
}

using System;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Common;
using StoreManagement.Data;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.ViewModels;

namespace StoreManagement.DataAccess.Services;

public class DashboardService(DatabaseContext ctx) : IDashboardService
{
      readonly DatabaseContext context = ctx;
      public async Task<DashboardStatsVM> GetDashboardStatsAsync()
      {
            var stats = new DashboardStatsVM
            {
                  // Get total products

                  TotalProducts = await context.Products.CountAsync(),

                  // Get total customers
                  TotalCustomers = await context.Customers.CountAsync(),

                  // Get total orders
                  TotalOrders = await context.Orders.CountAsync(),

                  // Get total revenue (sum of order totals)
                  TotalRevenue = await context.Orders
                      .Where(o => o.Status == OrderStatus.Delivered.ToDisplayName())
                      .SumAsync(o => o.TotalAmount),
            };

            // Get low stock products (assuming we have QuantityInStock property)
            var lowStockThreshold = 10; // You can make this configurable
            stats.LowStockProducts = await context.Products
                .Where(p => p.StockQuantity <= lowStockThreshold)
                .CountAsync();

            // Get low stock products list
            stats.LowStockProductsList = await context.Products
                .Where(p => p.StockQuantity <= lowStockThreshold)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .OrderBy(p => p.StockQuantity)
                .Take(10)
                .ToListAsync();

            // Get pending orders count
            stats.PendingOrders = await context.Orders
                .Where(o => o.Status == OrderStatus.Pending.ToDisplayName() ||
                           o.Status == OrderStatus.Processing.ToDisplayName())
                .CountAsync();

            // Get recent orders
            stats.RecentOrders = await context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            return stats;
      }
}

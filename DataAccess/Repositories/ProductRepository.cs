using System;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Data;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.Models;

namespace StoreManagement.DataAccess.Repositories;

public class ProductRepository(DatabaseContext ctx) : Repository<Product>(ctx), IProductRepository
{
      readonly DatabaseContext context = ctx;
      public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
      {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.StockQuantity <= threshold && p.IsActive)
                .OrderBy(p => p.StockQuantity)
                .ToListAsync();
      }

      public async Task<IEnumerable<Product>> GetProductsWithDetailsAsync()
      {
            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderBy(p => p.Name)
                .ToListAsync();
      }

      public async Task<Product?> GetProductWithDetailsAsync(int id)
      {
            return await context.Products
                 .Include(p => p.Category)
                 .Include(p => p.Brand)
                 .FirstOrDefaultAsync(p => p.Id == id);
      }

      public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
      {
            if (string.IsNullOrWhiteSpace(searchTerm))
                  return await GetProductsWithDetailsAsync();

            return await context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Name.Contains(searchTerm) ||
                           (p.Description ?? "").Contains(searchTerm))
                .OrderBy(p => p.Name)
                .ToListAsync();
      }
}

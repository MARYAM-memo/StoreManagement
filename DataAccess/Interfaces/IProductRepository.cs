using System;
using StoreManagement.Models;

namespace StoreManagement.DataAccess.Interfaces;

public interface IProductRepository : IRepository<Product>
{
      Task<IEnumerable<Product>> GetProductsWithDetailsAsync();
      Task<Product?> GetProductWithDetailsAsync(int id);
      Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
      Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
}

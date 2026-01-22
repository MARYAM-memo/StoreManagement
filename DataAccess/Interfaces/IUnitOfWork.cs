using System;
using StoreManagement.Models;
using StoreManagement.Models.Inventory;
namespace StoreManagement.DataAccess.Interfaces;

public interface IUnitOfWork : IDisposable
{
      IRepository<Category> Categories { get; }
      IRepository<Brand> Brands { get; }
      IProductRepository Products { get; }
      IRepository<Order> Orders { get; }
      IRepository<Customer> Customers { get; }
      IRepository<OrderItem> OrderItems { get; }
      IRepository<Supplier> Suppliers { get; }
      IRepository<StockTransaction> StockTransactions { get; }

      int SaveChanges();
      Task<int> SaveChangesAsync();
}

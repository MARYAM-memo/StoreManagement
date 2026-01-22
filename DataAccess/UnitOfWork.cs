using System;
using StoreManagement.Data;
using StoreManagement.DataAccess.Interfaces;
using StoreManagement.DataAccess.Repositories;
using StoreManagement.Models;
using StoreManagement.Models.Inventory;

namespace StoreManagement.DataAccess;

public class UnitOfWork : IUnitOfWork
{
      readonly DatabaseContext context;
      public UnitOfWork(DatabaseContext ctx)
      {
            context = ctx;
            Categories = new Repository<Category>(context);
            Brands = new Repository<Brand>(context);
            Orders = new Repository<Order>(context);
            Customers = new Repository<Customer>(context);
            OrderItems = new Repository<OrderItem>(context);
            Products = new ProductRepository(context);
            Suppliers = new Repository<Supplier>(context);
            StockTransactions = new Repository<StockTransaction>(context);
      }
      public IRepository<Category> Categories { get; private set; }

      public IRepository<Brand> Brands { get; private set; }


      public IRepository<Order> Orders { get; private set; }

      public IRepository<Customer> Customers { get; private set; }

      public IRepository<OrderItem> OrderItems { get; private set; }

      public IProductRepository Products { get; private set; }

      public IRepository<Supplier> Suppliers { get; set; }

      public IRepository<StockTransaction> StockTransactions { get; set; }

      public int SaveChanges()
      {
            return context.SaveChanges();
      }
      public async Task<int> SaveChangesAsync()
      {
            return await context.SaveChangesAsync();
      }

      public void Dispose()
      {
            context.Dispose();
            GC.SuppressFinalize(this);
      }
}

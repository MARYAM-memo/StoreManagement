using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StoreManagement.Data;
using StoreManagement.DataAccess.Interfaces;

namespace StoreManagement.DataAccess.Repositories;

public class Repository<T>(DatabaseContext ctx) : IRepository<T> where T : class
{
      readonly DatabaseContext context = ctx;

      public void Add(T entity)
      {
            context.Set<T>().Add(entity);
      }
      public async Task AddAsync(T entity)
      {
            await context.Set<T>().AddAsync(entity);
      }

      public IEnumerable<T> FetchAll()
      {
            return [.. context.Set<T>()];
      }

      public async Task<IEnumerable<T>> FetchAllAsync()
      {
            var list = await context.Set<T>().ToListAsync();
            return list;
      }

      public IEnumerable<T> FetchAll(params Expression<Func<T, object>>[] args)
      {
            IQueryable<T> query = context.Set<T>();
            foreach (var arg in args)
            {
                  query = query.Include(arg);
            }
            return [.. query];
      }

      public async Task<IEnumerable<T>> FetchAllAsync(params Expression<Func<T, object>>[] args)
      {
            IQueryable<T> query = context.Set<T>();
            foreach (var arg in args)
            {
                  query = query.Include(arg);
            }
            return await query.ToListAsync();
      }

      public T? Find(Func<T, bool> match)
      {
            return context.Set<T>().FirstOrDefault(match);
      }
      public async Task<T?> FindAsync(Expression<Func<T, bool>> match)
      {

            return await context.Set<T>().FirstOrDefaultAsync(match);
      }

      public T? Find(int id)
      {
            return context.Set<T>().Find(id);
      }

      public async Task<T?> FindAsync(int id)
      {
            return await context.Set<T>().FindAsync(id);
      }

      public void Remove(T entity)
      {
            context.Set<T>().Remove(entity);
      }

      public void Update(T entity)
      {
            context.Set<T>().Update(entity);
      }
}

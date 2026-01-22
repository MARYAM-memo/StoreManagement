using System;
using System.Linq.Expressions;

namespace StoreManagement.DataAccess.Interfaces;

public interface IRepository<T> where T : class
{
      IEnumerable<T> FetchAll(); 
      Task<IEnumerable<T>> FetchAllAsync();
      IEnumerable<T> FetchAll(params Expression<Func<T, object>>[] args); 
      Task<IEnumerable<T>> FetchAllAsync(params Expression<Func<T, object>>[] args);
      T? Find(Func<T, bool> match); 
      Task<T?> FindAsync(Expression<Func<T, bool>> match);
      T? Find(int id); 
      Task<T?> FindAsync(int id);
      void Add(T entity);
      Task AddAsync(T entity);
      void Update(T entity);
      void Remove(T entity);
}

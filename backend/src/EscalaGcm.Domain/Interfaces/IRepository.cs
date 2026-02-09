using System.Linq.Expressions;
using EscalaGcm.Domain.Common;

namespace EscalaGcm.Domain.Interfaces;

public interface IRepository<T> where T : AuditableEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> SaveChangesAsync();
}

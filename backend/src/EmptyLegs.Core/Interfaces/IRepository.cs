using EmptyLegs.Core.Entities;
using System.Linq.Expressions;

namespace EmptyLegs.Core.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetPagedAsync<TKey>(int page, int pageSize, Expression<Func<T, TKey>> orderBy, bool ascending = true, CancellationToken cancellationToken = default);
}
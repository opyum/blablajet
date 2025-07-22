using EmptyLegs.Core.Entities;
using EmptyLegs.Core.Interfaces;
using EmptyLegs.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmptyLegs.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly EmptyLegsDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(EmptyLegsDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => !e.IsDeleted && e.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(e => !e.IsDeleted).AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => !e.IsDeleted);
        
        if (predicate == null)
            return await query.CountAsync(cancellationToken);
        
        return await query.CountAsync(predicate, cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        var entitiesList = entities.ToList();
        await _dbSet.AddRangeAsync(entitiesList, cancellationToken);
        return entitiesList;
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            Update(entity);
        }
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => !e.IsDeleted)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync<TKey>(
        int page, 
        int pageSize, 
        Expression<Func<T, TKey>> orderBy, 
        bool ascending = true, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(e => !e.IsDeleted);
        
        query = ascending 
            ? query.OrderBy(orderBy) 
            : query.OrderByDescending(orderBy);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
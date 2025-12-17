using FaqAssistant.Application.Common;
using FaqAssistant.Application.Interfaces.Repositories;
using FaqAssistant.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FaqAssistant.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _dbSet = _dbContext.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedAsync(int pageNumber, int pageSize, 
    Expression<Func<T, bool>> predicate, 
    CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        var query = _dbSet.AsNoTracking().Where(predicate);
        var totalCount = await query.CountAsync(cancellationToken);
        var items =  await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<T>.Create(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }
    public void DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public void DeleteAsync(List<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken) != null;
    }

    public async Task<PagedResult<T>> GetPagedAsync(
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbSet.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResult<T>.Create(
            items,
            pageNumber,
            pageSize,
            totalCount);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }
}

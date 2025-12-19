using System.Linq.Expressions;
using FaqAssistant.Application.Common;

namespace FaqAssistant.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedResult<T>> GetPagedAsync(
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        void DeleteAsync(T entity);
        void DeleteAsync(List<T> entities);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync();

    }
}

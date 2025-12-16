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
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}

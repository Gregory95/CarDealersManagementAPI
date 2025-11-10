using PagingPackage;

namespace CarsDealersManagement.Domain.Repositories
{
    public interface IBaseRepository<T>
    {
        Task<T?> GetByIdAsync(int Id, CancellationToken ct);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct);
        Task AddAsync(T message, CancellationToken ct);
        Task UpdateAsync(T message, CancellationToken ct);
        Task DeleteAsync(int Id, CancellationToken ct);
        void Delete(int Id);
        Task<PagingToolkit<T>> GetPagedAsync(IQueryable<T> query, int pageNumber, int pageSize, CancellationToken ct);
    }
}

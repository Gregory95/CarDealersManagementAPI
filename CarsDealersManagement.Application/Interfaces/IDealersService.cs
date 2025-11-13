using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Application.Interfaces
{
    public interface IDealersService
    {
        Task<IEnumerable<DealersDto>> GetDetailsAsync(CancellationToken ct);

        Task<PagingWrap<DealersDto>> GetDealersPagingAsync(PagingRequest parameters, CancellationToken ct);
    }
}

using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Application.Interfaces
{
    public interface IDealersService
    {
        Task<IEnumerable<DealersDto>> GetDetailsAsync(CancellationToken ct);

        Task CreateDealerAsync(DealersDto dto, CancellationToken ct);

        Task<DealersDto> EditDealerAsync(DealersDto dto, CancellationToken ct);

        Task<DealersDto> GetDealerByIdAsync(int Id, CancellationToken ct);

        Task DeleteDealerAsync(int Id, CancellationToken ct);

        Task<PagingWrap<Dealer>> GetDealersPagingAsync(PagingRequest parameters, CancellationToken ct);
    }
}

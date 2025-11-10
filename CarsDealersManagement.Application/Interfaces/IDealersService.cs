using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using PagingPackage;

namespace CarsDealersManagement.Application.Interfaces
{
    public interface IDealersService
    {
        Task<IEnumerable<DealersDto>> GetDetailsAsync(CancellationToken ct);

        Task<PagingToolkit<DealersDto>> GetDealersPagingAsync(PagingParameters parameters, CancellationToken ct);
    }
}

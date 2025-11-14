using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using Pagination.Query.EntityFramework;

namespace CarsShowroomsManagement.Application.Interfaces
{
    public interface IShowroomsService
    {
        Task<IEnumerable<ShowroomDto>> GetDetailsAsync(CancellationToken ct);

        Task CreateShowroomAsync(ShowroomDto dto, CancellationToken ct);

        Task<ShowroomDto> EditShowroomAsync(ShowroomDto dto, CancellationToken ct);

        Task<ShowroomDto> GetShowroomByIdAsync(int Id, CancellationToken ct);

        Task DeleteShowroomAsync(int Id, CancellationToken ct);

        Task<PagingWrap<Showroom>> GetShowroomsPagingAsync(PagingRequest parameters, CancellationToken ct);
    }
}

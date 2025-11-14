using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Application.Interfaces
{
    public interface IContactPersonsService
    {
        Task<IEnumerable<ContactPersonDto>> GetDetailsAsync(CancellationToken ct);

        Task CreateContactPersonAsync(ContactPersonDto dto, CancellationToken ct);

        Task<ContactPersonDto> EditContactPersonAsync(ContactPersonDto dto, CancellationToken ct);

        Task<ContactPersonDto> GetContactPersonByIdAsync(int Id, CancellationToken ct);

        Task DeleteContactPersonAsync(int Id, CancellationToken ct);

        Task<PagingWrap<ContactPerson>> GetContactPersonsPagingAsync(PagingRequest parameters, CancellationToken ct);
    }
}

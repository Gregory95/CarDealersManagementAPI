using AutoMapper;
using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Domain.Repositories;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Application.Services
{
    public class ContactPersonsService(IContactPersonsRepository _repo, IMapper _mapper) : IContactPersonsService
    {
        public async Task<PagingWrap<ContactPerson>> GetContactPersonsPagingAsync(PagingRequest parameters, CancellationToken ct)
        {
            return await _repo.GetPagedAsync(_repo.GetAll(), parameters.PageNumber, parameters.PageSize, ct);
        }

        public async Task<IEnumerable<ContactPersonDto>> GetDetailsAsync(CancellationToken ct)
        {
            var results = await _repo.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<ContactPersonDto>>(results);
        }

        public async Task CreateContactPersonAsync(ContactPersonDto dto, CancellationToken ct)
        {
            await _repo.AddAsync(_mapper.Map<ContactPerson>(dto), ct);
        }

        public async Task<ContactPersonDto> EditContactPersonAsync(ContactPersonDto dto, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync((int)dto.Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Contact person not found");
            }

            entity.Update(dto.FirstName, dto.LastName, dto.Email, dto.PhoneNumber);
            await _repo.UpdateAsync(entity, ct);
            return _mapper.Map<ContactPersonDto>(entity);
        }

        public async Task<ContactPersonDto> GetContactPersonByIdAsync(int Id, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Contact person not found");
            }

            return _mapper.Map<ContactPersonDto>(entity);
        }

        public async Task DeleteContactPersonAsync(int Id, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Contact person not found");
            }

            await _repo.DeleteAsync(Id, ct);
        }
    }
}

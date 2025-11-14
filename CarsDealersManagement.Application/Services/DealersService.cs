using AutoMapper;
using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Domain.Repositories;
using Pagination.Query.EntityFramework;


namespace CarsDealersManagement.Application.Services
{
    public class DealersService(IDealersRepository _repo, IMapper _mapper) : IDealersService
    {
        public async Task<PagingWrap<Dealer>> GetDealersPagingAsync(PagingRequest parameters, CancellationToken ct)
        {
            return  await _repo.GetPagedAsync(_repo.GetAll(), parameters.PageNumber, parameters.PageSize, ct);
        }

        public async Task<IEnumerable<DealersDto>> GetDetailsAsync(CancellationToken ct)
        {
            var results = await _repo.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<DealersDto>>(results);
        }

        public async Task CreateDealerAsync(DealersDto dto, CancellationToken ct)
        {
            await _repo.AddAsync(_mapper.Map<Dealer>(dto), ct);
        }

        public async Task<DealersDto> EditDealerAsync(DealersDto dto, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync((int)dto.Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Dealer not found");
            }

            entity.Update(dto.FirstName, dto.LastName, dto.Email, dto.PhoneNumber);
            await _repo.UpdateAsync(entity, ct);
            return _mapper.Map<DealersDto>(entity);
        }

        public async Task<DealersDto> GetDealerByIdAsync(int Id, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Dealer not found");
            }

            return _mapper.Map<DealersDto>(entity);
        }

        public async Task DeleteDealerAsync(int Id, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Dealer not found");
            }

            await _repo.DeleteAsync(Id, ct);
        }
    }
}

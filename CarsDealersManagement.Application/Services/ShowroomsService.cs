using AutoMapper;
using CarsShowroomsManagement.Application.Interfaces;
using Pagination.Query.EntityFramework;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Repositories;
using CarsDealersManagement.Domain.Models;


namespace CarsShowroomsManagement.Application.Services
{
    public class ShowroomsService(IShowroomsRepository _repo, IMapper _mapper) : IShowroomsService
    {
        public async Task<PagingWrap<Showroom>> GetShowroomsPagingAsync(PagingRequest parameters, CancellationToken ct)
        {
            return  await _repo.GetPagedAsync(_repo.GetAll(), parameters.PageNumber, parameters.PageSize, ct);
        }

        public async Task<IEnumerable<ShowroomDto>> GetDetailsAsync(CancellationToken ct)
        {
            var results = await _repo.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<ShowroomDto>>(results);
        }

        public async Task CreateShowroomAsync(ShowroomDto dto, CancellationToken ct)
        {
            await _repo.AddAsync(_mapper.Map<Showroom>(dto), ct);
        }

        public async Task<ShowroomDto> EditShowroomAsync(ShowroomDto dto, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync((int)dto.Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Showroom not found");
            }

            entity.Update(dto.Name, dto.Location, dto.Capacity);
            await _repo.UpdateAsync(entity, ct);
            return _mapper.Map<ShowroomDto>(entity);
        }

        public async Task<ShowroomDto> GetShowroomByIdAsync(int Id, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Showroom not found");
            }

            return _mapper.Map<ShowroomDto>(entity);
        }

        public async Task DeleteShowroomAsync(int Id, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(Id, ct);

            if (entity == null)
            {
                throw new KeyNotFoundException("Showroom not found");
            }

            await _repo.DeleteAsync(Id, ct);
        }
    }
}

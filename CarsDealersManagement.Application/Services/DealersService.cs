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
        public async Task<PagingWrap<DealersDto>> GetDealersPagingAsync(PagingRequest parameters, CancellationToken ct)
        {
            var results =  await _repo.GetPagedAsync(_repo.GetAll(), parameters.PageNumber, parameters.PageSize, ct);
            return _mapper.Map<PagingWrap<DealersDto>>(results);
        }

        public async Task<IEnumerable<DealersDto>> GetDetailsAsync(CancellationToken ct)
        {
            var results = await _repo.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<DealersDto>>(results);
        }
    }
}

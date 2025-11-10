using AutoMapper;
using CarsDealersManagement.Application.Interfaces;
using CarsDealersManagement.Domain.Models;
using CarsDealersManagement.Domain.Repositories;
using PagingPackage;

namespace CarsDealersManagement.Application.Services
{
    public class DealersService(IDealersRepository _repo, IMapper _mapper) : IDealersService
    {
        public async Task<PagingToolkit<DealersDto>> GetDealersPagingAsync(PagingParameters parameters, CancellationToken ct)
        {
            var results =  _repo.GetPagedAsync(_repo.GetAll(), parameters.PageNumber, parameters.PageSize, ct);
            return _mapper.Map<PagingToolkit<DealersDto>>(results);
        }

        public async Task<IEnumerable<DealersDto>> GetDetailsAsync(CancellationToken ct)
        {
            var results = await _repo.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<DealersDto>>(results);
        }
    }
}

using AutoMapper;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;
using Pagination.Query.EntityFramework;

namespace CarsDealersManagement.Application
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Dealer, DealersDto>().ReverseMap();
            CreateMap<PagingWrap<Dealer>, PagingWrap<DealersDto>>().ReverseMap();
        }
    }
}

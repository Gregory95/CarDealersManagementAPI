using AutoMapper;
using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Models;

namespace CarsDealersManagement.Application
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Dealer, DealersDto>().ReverseMap();
            CreateMap<ContactPerson, ContactPersonDto>().ReverseMap();
            CreateMap<Showroom, ShowroomDto>().ReverseMap();
        }
    }
}

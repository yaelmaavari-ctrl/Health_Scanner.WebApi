using AutoMapper;
using Repository.Entities;
using Service.Dto;

namespace Service
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<AllergenCreateDto, Allergen>();
            CreateMap<Allergen, AllergenDto>();
            CreateMap<ScanHistory, ScanHistoryDto>();
        }
    }
}
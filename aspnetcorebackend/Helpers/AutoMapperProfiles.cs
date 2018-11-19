using aspnetcorebackend.Models.Dtos;
using aspnetcorebackend.Models.Entities;
using AutoMapper;

namespace aspnetcorebackend.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
        }
    }
}
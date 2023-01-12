using AutoMapper;
using BlogApp.DTOs;
using BlogApp.Entities;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDTO>();
        }
    }
}
using AutoMapper;

namespace Nexus.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Models.User, DTOs.UserDto>();
            CreateMap<DTOs.CreateUserDto, Models.User>();
        }
    }
}
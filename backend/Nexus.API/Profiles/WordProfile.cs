using AutoMapper;

namespace Nexus.API.Profiles
{
    public class WordProfile : Profile
    {
        public WordProfile()
        {
            CreateMap<Models.Word, DTOs.WordDto>();
            CreateMap<DTOs.CreateWordDto, Models.Word>();
        }
    }
}
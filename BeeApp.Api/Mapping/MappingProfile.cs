using AutoMapper;
using BeeApp.Shared.DTO;
using BeeApp.Shared.Models;

namespace BeeApp.Api.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Apiary, ApiaryDto>().ReverseMap();
            CreateMap<Hive, HiveDto>().ReverseMap();
            // others future mapping
        }
    }
}

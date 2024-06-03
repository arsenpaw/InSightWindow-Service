using AutoMapper;
using InSightWindowAPI.Models;

namespace InSightWindowAPI
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<User, User>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.Ignore())
                    .ForMember(
                 dest => dest.Devices,
                        opt => opt.Ignore());
        }
    }
}

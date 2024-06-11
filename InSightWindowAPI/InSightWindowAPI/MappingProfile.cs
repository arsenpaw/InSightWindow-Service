using AutoMapper;
using InSightWindowAPI.Models;

namespace InSightWindowAPI
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegister, UserRegister>()
                    .ForMember(
                        dest => dest.Id,
                        opt => opt.Ignore())
                    .ForMember(
                 dest => dest.Devices,
                        opt => opt.Ignore());
            CreateMap<Device, Device>()
                .ForMember(
                dest => dest.Id,
                opt => opt.Ignore());


        }
    }
}

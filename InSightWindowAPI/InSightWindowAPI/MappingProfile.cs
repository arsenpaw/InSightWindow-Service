using AutoMapper;
using InSightWindowAPI.Models;
using InSightWindowAPI.Models.Dto;
using System.Diagnostics;

namespace InSightWindowAPI
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
          
            CreateMap<Device, Device>()
                .ForMember(
                dest => dest.Id,
                opt => opt.Ignore());
            CreateMap<UserRegisterDto, UserDto>();        
            CreateMap<Device, DeviceDto>();
            CreateMap<UserDto, UserRegisterDto>();
            CreateMap<UserRegisterDto, User>();

        }
    }
}

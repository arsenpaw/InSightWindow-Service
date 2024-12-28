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
            CreateMap<User, UserDto>();        
            CreateMap<UserDto, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Device, DeviceDto>();
            CreateMap<DeviceDto, Device>();
            CreateMap<RefreshToken, RefreshToken>()
                .ForMember( dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore()); 
            ;
        }
    }
}

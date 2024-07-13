using AutoMapper;
using InSightWindowAPI.Models;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Models.Dto;
using System.Diagnostics;
using InSightWindow.Models;
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
                  
         //   CreateMap<List<Device>,List<DeviceDto>>();
            CreateMap<DeviceDto, Device>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<RefreshToken, RefreshToken>()
                .ForMember( dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore()); 
            ;
        }
    }
}

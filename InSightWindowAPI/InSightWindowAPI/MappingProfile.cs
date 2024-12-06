using AutoMapper;
using InSightWindow.Models;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Models.Dto;
using InSightWindowAPI.Models.Entity;
namespace InSightWindowAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Device, Device>()
                .ForMember(
                dest => dest.Id,
                opt => opt.Ignore());
            CreateMap<User, UserRegisterDto>();
            CreateMap<UserRegisterDto, UserRegisterDto>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<Device, DeviceDto>();
            CreateMap<DeviceDto, Device>()

           ////   CreateMap<List<Device>,List<DeviceDto>>();
           //   CreateMap<DeviceDto, Device>()
           //        .ForMember(dest => dest.Id, opt => opt.Ignore());
           //   CreateMap<RefreshToken, RefreshToken>()
           //       .ForMember( dest => dest.UserId, opt => opt.Ignore())
           //       .ForMember(dest => dest.Id, opt => opt.Ignore())
           //       .ForMember(dest => dest.User, opt => opt.Ignore()); 
           ;
        }
    }
}

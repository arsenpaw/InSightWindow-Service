using AutoMapper;
using InSightWindow.Models;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InSightWindowAPI.Serivces
{
    public class DeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;


        public DeviceService(IDeviceRepository deviceRepository, IMapper mapper)
        {
            _mapper = mapper;
            _deviceRepository = deviceRepository;   
        }

        public async Task<DeviceDto> GetDeviceById(Guid deviceId)
        {
            var device = _getDevice(deviceId);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> AddDeviceToUser(Guid userId, Guid deviceId) 
        {
            //get user
            var device =await  _getDevice(deviceId);
            if (device.UserId is not null)
            {
                //Throw execption
            }    
        }

        public async Task<IEnumerable<DeviceDto>> GetUserDevices(Guid userId)
        {
            var userDevices = await _getDevices(userId);
             return  _mapper.Map<IEnumerable<DeviceDto>>(userDevices);
        }

        private async Task<IEnumerable<Device>> _getDevices(Guid userId)
        {
            var devices = await _deviceRepository.GetByUserId(userId).ToListAsync();
            if (devices is null)
                throw new Exception();
            return devices;
        }

        private async Task<Device> _getDevice(Guid deviceId)
        {
            var device = await  _deviceRepository.GetAll()
                .Include(x => x.User)
                .Where(x => x.Id == deviceId)
                .FirstOrDefaultAsync();
            if (device is null)
            {
                throw new Exception();
                //TODO app exeption
            }
            return device;
        }

    }
}

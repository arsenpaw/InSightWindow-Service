using AutoMapper;
using Domain.Entity.DeviceModel;
using InSightWindow.Models;
using InSightWindowAPI.Exceptions;
using InSightWindowAPI.Exeptions;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InSightWindowAPI.Serivces
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;


        public DeviceService(IDeviceRepository deviceRepository, IMapper mapper)
        {
            _mapper = mapper;
            _deviceRepository = deviceRepository;
        }


        public async Task<DeviceDto> CreateDevice(DeviceDto deviceDto)
        {
            var device = _mapper.Map<Device>(deviceDto);
            await _deviceRepository.AddAsync(device);
            await _deviceRepository.SaveAsync();
            return _mapper.Map<DeviceDto>(device);
        }
        public async Task DeleteDevice(Guid deviceId)
        {
            var device = await _getDevice(deviceId);
            if (device.UserId is not null)
            {
                throw new AppException(ExceptionMessages.DeviceOccupied);
            }
            _deviceRepository.RemoveAsync(device);
            await _deviceRepository.SaveAsync();
        }

        public async Task<DeviceDto> GetDeviceById(Guid deviceId)
        {
            var device = await _getDevice(deviceId);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto> RemoveDeviceFromUser(Guid userId, Guid deviceId)
        {
            var device = await _getUserDevice(deviceId, userId);
            device.UserId = null;
            await _deviceRepository.SaveAsync();
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task AddDeviceToUser(Guid userId, Guid deviceId)
        {
            var device = await _getDevice(deviceId);
            if (device.UserId is not null)
            {
                throw new AppException(ExceptionMessages.DeviceOccupied);
            }
            device.UserId = userId;
            await _deviceRepository.SaveAsync();
        }

        public async Task<IEnumerable<DeviceDto>> GetUserDevices(Guid userId)
        {
            var userDevices = await _getUserDevices(userId);
            return _mapper.Map<IEnumerable<DeviceDto>>(userDevices);
        }

        private async Task<IEnumerable<Device>> _getUserDevices(Guid userId)
        {
            var devices = await _deviceRepository.GetByUserId(userId).ToListAsync();
            return devices;
        }
        private async Task<Device> _getUserDevice(Guid deviceId, Guid userId)
        {
            var userDevice = await _deviceRepository
                .GetByUserId(userId)
                .Where(x => x.Id == deviceId)
                .FirstOrDefaultAsync();
            if (userDevice is null)
            {
                throw new AppException(ExceptionMessages.NoSuchDeviceExist);
            }
            return userDevice;
        }
        private async Task<Device> _getDevice(Guid deviceId)
        {
            var device = await _deviceRepository.GetAll()
                .Include(x => x.User)
                .Where(x => x.Id == deviceId)
                .FirstOrDefaultAsync();
            if (device is null)
            {
                throw new AppException(ExceptionMessages.NoSuchDeviceExist);
            }
            return device;
        }

    }
}

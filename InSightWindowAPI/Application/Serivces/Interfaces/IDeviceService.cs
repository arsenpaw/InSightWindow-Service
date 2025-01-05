using InSightWindow.Models;

namespace InSightWindowAPI.Serivces
{
    public interface IDeviceService
    {
        Task AddDeviceToUser(Guid userId, Guid deviceId);
        Task<DeviceDto> CreateDevice(DeviceDto deviceDto);
        Task DeleteDevice(Guid deviceId);
        Task<DeviceDto> GetDeviceById(Guid deviceId);
        Task<IEnumerable<DeviceDto>> GetUserDevices(Guid userId);
        Task<DeviceDto> RemoveDeviceFromUser(Guid userId, Guid deviceId);
    }
}
using InSightWindowAPI.Models;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Repository.Interfaces;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace InSightWindowAPI.Repository
{
    public class DeviceRespository : BaseRepository, IDeviceRepository
    {
        public DeviceRespository(UsersContext context): base(context)
        {

        }

        public IQueryable<Device> GetAll()
        => _context.Devices;

        public IQueryable<Device> GetById(Guid deviceId) => _context.Devices.Where(x => x.Id == deviceId);
            
        public IQueryable<Device> GetByUserId(Guid userId) => _context.Devices.Where(x => x.UserId == userId);

        public void Add(Device device) =>  _context.Devices.Add(device);

        public void BindToUser(Device device, Guid userId)
        {
            device.UserId = userId;
            _context.Devices.Update(device);
        }

        public void UnBindFromUser(Device device)
        {
            device.UserId = null;
            _context.Devices.Update(device);
        }
    }
}

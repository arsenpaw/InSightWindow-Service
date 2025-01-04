using Domain.Entity.DeviceModel;
using Infrastructure.Data;
using Infrastructure.Repository;
using InSightWindowAPI.Models;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Repository.Interfaces;

namespace InSightWindowAPI.Repository
{
    public class DeviceRepository : BaseRepository, IDeviceRepository
    {
        public DeviceRepository(UsersContext context) : base(context)
        {

        }

        public IQueryable<Device> GetAll()
        => _context.Devices;

        public IQueryable<Device> GetById(Guid deviceId) => _context.Devices.Where(x => x.Id == deviceId);

        public IQueryable<Device> GetByUserId(Guid userId) => _context.Devices.Where(x => x.UserId == userId);

        public async Task AddAsync(Device device) => await _context.Devices.AddAsync(device);

        public void RemoveAsync(Device device) => _context.Devices.Remove(device);

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

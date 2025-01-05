using Domain.Entity.DeviceModel;
using InSightWindowAPI.Models.DeviceModel;

namespace InSightWindowAPI.Repository.Interfaces
{
    public interface IDeviceRepository: IBaseRepository
    {
        Task AddAsync(Device device);
        void BindToUser(Device device, Guid userId);
        IQueryable<Device> GetAll();
        IQueryable<Device> GetById(Guid deviceId);
        IQueryable<Device> GetByUserId(Guid userId);
        void RemoveAsync(Device device);
        void UnBindFromUser(Device device);
    }
}
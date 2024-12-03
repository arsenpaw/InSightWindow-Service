using InSightWindowAPI.Models.DeviceModel;

namespace InSightWindowAPI.Repository.Interfaces
{
    public interface IDeviceRepository: IBaseRepository
    {
        IQueryable<Device> GetAll();

        public IQueryable<Device> GetById(Guid deviceId);

        public IQueryable<Device> GetByUserId(Guid userId);

    }
}

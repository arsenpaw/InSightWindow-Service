using InSightWindowAPI.Models.Dto;

namespace InSightWindowAPI.SensorDataProcessors.Interfaces
{
    public interface ISensorDataProcessor<T>
    {
        Task ProcessDataAsync(T data, Guid userId, Guid deviceId);
    }
}
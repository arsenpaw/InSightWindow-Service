using Application.Serivces;
using Domain.IRepository;
using Infrastructure.Repository;
using InSightWindowAPI.Models.Sensors;
using InSightWindowAPI.Repository;
using InSightWindowAPI.Repository.Interfaces;
using InSightWindowAPI.SensorDataProcessors;
using InSightWindowAPI.SensorDataProcessors.Interfaces;
using InSightWindowAPI.Serivces;
using InSightWindowAPI.Serivces.Interfaces;

namespace InSightWindowAPI.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataProcessors(this IServiceCollection Services)
        {
            Services.AddTransient<ISensorDataProcessor<AlarmSensor>, AlarmDataProcessor>();

            return Services;
        }
        public static IServiceCollection AddServices(this IServiceCollection Services)
        {
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddScoped<IDeviceService, DeviceService>();
            Services.AddScoped<IPushNotificationService, PushNotificationService>();
            Services.AddTransient<IFireBaseTokenService, FireBaseTokenService>();
            Services.AddTransient<IAesService>(provider =>
                               new AesService("1234567890ABCDEF", "1234567890ABCDEF"));
            return Services;
        }
        public static IServiceCollection AddRepository(this IServiceCollection Services)
        {
            Services.AddScoped<IDeviceRepository, DeviceRepository>();
            Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            Services.AddScoped<IFireBaseRepository, FireBaseRepository>();
            return Services;
        }


    }
}

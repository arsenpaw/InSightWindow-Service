using InSightWindowAPI.Repository;
using InSightWindowAPI.Repository.Interfaces;
using InSightWindowAPI.Serivces;
using InSightWindowAPI.Serivces.Interfaces;
using InSightWindowAPI.Services;

namespace InSightWindowAPI.Extensions
{
    public static  class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServices (this IServiceCollection Services)
        {
            Services.AddScoped<ITokenService, TokenService>();
            Services.AddScoped<IDeviceService, DeviceService>();
            Services.AddTransient<IPushNotificationService, PushNotificationService>();
            Services.AddTransient<IAesService>(provider =>
                               new AesService("1234567890ABCDEF", "1234567890ABCDEF"));
            return Services;
        }
        public static IServiceCollection AddRepository(this IServiceCollection Services)
        {
            Services.AddTransient<IDeviceRepository, DeviceRepository>();
            Services.AddTransient<IUserRepository, UserRepository>();   
            return Services;
        }


    }
}

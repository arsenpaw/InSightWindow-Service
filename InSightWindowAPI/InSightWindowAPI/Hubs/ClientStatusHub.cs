using InSightWindowAPI.Hubs.ConnectionMapper;
using InSightWindowAPI.Models.Command;
using InSightWindowAPI.Models.Dto.ESP32;
using InSightWindowAPI.Models.Sensors;
using InSightWindowAPI.Repository.Interfaces;
using InSightWindowAPI.SensorDataProcessors.Interfaces;
using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net;

namespace InSightWindowAPI.Hubs
{
    [AllowAnonymous]
    public class ClientStatusHub : BaseHub
    {
        private IMemoryCache _cache;
        private ILogger<ClientStatusHub> _logger;
        private readonly IPushNotificationService _pusher;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ISensorDataProcessor<AlarmSensor> _alarmDataProcessor;
        private readonly static ConnectionMapping<Guid> _connectionMapping =
           new ConnectionMapping<Guid>();

        public ClientStatusHub(
            ISensorDataProcessor<AlarmSensor> alarmDataProcessor,
            IDeviceRepository context,
       IMemoryCache memoryCache,
       ILogger<ClientStatusHub> logger,
       IPushNotificationService pusher,
       IAesService aesService) : base(aesService)
        {
            _deviceRepository = context;
            _cache = memoryCache;
            _logger = logger;
            _pusher = pusher;
            _alarmDataProcessor = alarmDataProcessor;

        }

        public override Task OnConnectedAsync()
        {
            if (DeviceId != Guid.Empty)
            {
                _logger.Log(LogLevel.Information, "Device {i} connected to hub", DeviceId);
                _connectionMapping.Add(DeviceId, Context.ConnectionId);
            }
            else if (UserId != Guid.Empty)
            {
                _logger.Log(LogLevel.Information, "User {i} connected to hub", DeviceId);
            }
            else
            {
                _logger.Log(LogLevel.Information, "Unkonwn has connected to hub");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var deviceId in _connectionMapping.GetConnections(DeviceId))
            {
                _connectionMapping.Remove(DeviceId, Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task<HttpStatusCode> ReceiveDataFromEsp32(string sensorData)
        {
            byte[] sensorDataByte = Convert.FromBase64String(sensorData);
            string jsonData = AesService.DecryptStringFromBytes_Aes(sensorDataByte);

            _logger.Log(LogLevel.Information, jsonData);
            var sensorDataDto = JsonConvert.DeserializeObject<SensorDataDto>(jsonData);

            if (sensorDataDto is null || DeviceId == Guid.Empty)
            {
                _logger.Log(LogLevel.Critical,
                    "No all credentials have detected while receive data from esp32, Data: {sdata}, DeviceId {uId}", sensorDataDto, DeviceId);
                return HttpStatusCode.Unauthorized;
            }

            var targetDevice = await _deviceRepository.GetById(DeviceId)
                .FirstOrDefaultAsync();

            if (targetDevice is null || !targetDevice.UserId.HasValue)
            {
                return HttpStatusCode.NotFound;
            }

            _connectionMapping.Add(targetDevice.UserId.Value, Context.ConnectionId);
            await _alarmDataProcessor.ProcessDataAsync(new AlarmSensor { IsAlarm = sensorDataDto.IsAlarm },
                targetDevice.UserId.Value, DeviceId);

            await Clients.User(targetDevice.UserId.Value.ToString()).SendAsync("ReceiveSensorData", sensorDataDto);
            _logger.Log(LogLevel.Information, "Data was sucesfully send from user{userJWTId} to gadget {userInputStatus.DeviceId}",
                targetDevice.UserId, DeviceId);

            return HttpStatusCode.OK;
        }

        private async Task<HttpStatusCode> SendDataToDeviceAsync<T>(Guid deviceId, string eventName, T data)
        {
            try
            {
                var connectionID = _connectionMapping.GetConnections(deviceId).FirstOrDefault();
                if (connectionID == null)
                {
                    _logger.LogWarning("No connection found for device {DeviceId}", deviceId);
                    return HttpStatusCode.NotFound;
                }

                var encryptedData = AesService.EncryptStringToBytes_Aes(JsonConvert.SerializeObject(data));
                await Clients.Client(connectionID).SendAsync(eventName, Convert.ToBase64String(encryptedData));
                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while sending data to device {DeviceId}", deviceId);
                return HttpStatusCode.InternalServerError;
            }
        }

        [Authorize]
        public Task<HttpStatusCode> SetSettingsOnEsp32(Guid deviceId, UserSettings userSettings)
        {
            return SendDataToDeviceAsync(deviceId, "Settings", userSettings);
        }

        [Authorize]
        public Task<HttpStatusCode> SendCommandToEsp32(Guid deviceId, CommandDto command)
        {
            return SendDataToDeviceAsync(deviceId, "ReceiveCommand", command);
        }
    }
}
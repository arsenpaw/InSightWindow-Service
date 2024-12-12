using InSightWindowAPI.Hubs.ConnectionMapper;
using InSightWindowAPI.Models.Command;
using InSightWindowAPI.Models.Dto;
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
            if (DeviceId == Guid.Empty)
            {
                _logger.Log(LogLevel.Information, "No device was connected to hub");
                return base.OnConnectedAsync();
            }
            _logger.Log(LogLevel.Information, "Device {i} connected to hub", DeviceId);
            _connectionMapping.Add(DeviceId, Context.ConnectionId);
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
            Console.WriteLine(DeviceId);
            string jsonData = AesService.DecryptStringFromBytes_Aes(sensorDataByte);
            _logger.Log(LogLevel.Information, jsonData);
            var sensorDataDto = JsonConvert.DeserializeObject<SensorDataDto>(jsonData);
            if (sensorDataDto == null || DeviceId == Guid.Empty)
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

        [Authorize]
        public async Task<HttpStatusCode> SendCommandToEsp32(Guid deviceId, CommandDto command)
        {
            try
            {
                var connectionID = _connectionMapping.GetConnections(deviceId).FirstOrDefault();

                var encryptedCommand = AesService.EncryptStringToBytes_Aes(JsonConvert.SerializeObject(command));
                if (connectionID == null)
                {
                    _logger.Log(LogLevel.Warning, "No connection found for device {deviceId}", deviceId);
                    return HttpStatusCode.NotFound;
                }


                await Clients.Client(connectionID).SendAsync("ReceiveCommand", Convert.ToBase64String(encryptedCommand));
                return HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, ex.Message);
                return HttpStatusCode.InternalServerError;
            }
        }



    }
}

using InSightWindowAPI.Hubs.ConnectionMapper;
using InSightWindowAPI.Models.Dto.ESP32;
using InSightWindowAPI.Models.Sensors;
using InSightWindowAPI.Repository.Interfaces;
using InSightWindowAPI.SensorDataProcessors.Interfaces;
using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;

namespace InSightWindowAPI.Hubs
{
    [AllowAnonymous]
    public class DeviceHub : BaseHub
    {
        private ILogger<DeviceHub> _logger;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ISensorDataProcessor<AlarmSensor> _alarmDataProcessor;
        private readonly ConnectionMapping<Guid> _connectionMapping;
        private readonly IAesService AesService;

        public DeviceHub(
            ISensorDataProcessor<AlarmSensor> alarmDataProcessor,
            IDeviceRepository context,
            ConnectionMapping<Guid> connectionMapping,
            ILogger<DeviceHub> logger,
            IAesService aesService)
        {
            _connectionMapping = connectionMapping;
            _deviceRepository = context;
            _logger = logger;
            _alarmDataProcessor = alarmDataProcessor;
            AesService = aesService;

        }

        public override Task OnConnectedAsync()
        {
            _logger.Log(LogLevel.Information, "Device {i} connected to hub", DeviceId);
            _connectionMapping.Add(DeviceId, Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {

            _logger.LogInformation(exception, "Device disconected");
            _connectionMapping.Remove(DeviceId, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        private T? _decryptRequest<T>(string cryptedBase64)
        {
            byte[] sensorDataByte = Convert.FromBase64String(cryptedBase64);
            string jsonData = AesService.DecryptStringFromBytes_Aes(sensorDataByte);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public async Task<HttpStatusCode> ReceiveDataFromEsp32(string sensorData)
        {
            var sensorDataDto = _decryptRequest<SensorDataDto>(sensorData);
            _logger.Log(LogLevel.Information, "Data was received from device {DeviceId}", DeviceId);
            if (sensorDataDto is null)
            {
                return HttpStatusCode.Unauthorized;
            }

            var targetDevice = await _deviceRepository.GetById(DeviceId)
                .FirstOrDefaultAsync();

            if (targetDevice?.UserId == null)
            {
                return HttpStatusCode.NotFound;
            }

            await _alarmDataProcessor.ProcessDataAsync(new AlarmSensor { IsAlarm = sensorDataDto.IsAlarm },
                targetDevice.UserId.Value, DeviceId);

            await Clients.User(targetDevice.UserId.Value.ToString()).SendAsync("ReceiveSensorData", sensorDataDto);
            _logger.Log(LogLevel.Information,
                "Data was send from user {userJWTId} to gadget {userInputStatus.DeviceId}",
                        targetDevice.UserId, DeviceId);

            return HttpStatusCode.OK;
        }



    }
}
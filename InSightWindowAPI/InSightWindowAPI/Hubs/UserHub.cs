using InSightWindowAPI.Exeptions;
using InSightWindowAPI.Hubs.ConnectionMapper;
using InSightWindowAPI.Models.Command;
using InSightWindowAPI.Models.Dto.ESP32;
using InSightWindowAPI.Serivces.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Net;

namespace InSightWindowAPI.Hubs
{
    [AllowAnonymous]
    public class UserHub : BaseHub
    {
        private readonly ILogger<UserHub> _logger;
        private readonly ConnectionMapping<Guid> _connectionMapping;
        private readonly IAesService AesService;

        public UserHub(ConnectionMapping<Guid> connectionMapping, ILogger<UserHub> logger, IAesService aesService)
        {
            AesService = aesService;
            _logger = logger;
            _connectionMapping = connectionMapping;
        }
        public Task<HttpStatusCode> SetSettingsOnEsp32(Guid deviceId, UserSettings userSettings)
        {
            return SendDataToDeviceAsync(deviceId, "Settings", userSettings);
        }

        public Task<HttpStatusCode> SendCommandToEsp32(Guid deviceId, CommandDto command)
        {
            return SendDataToDeviceAsync(deviceId, "ReceiveCommand", command);
        }
        private async Task<HttpStatusCode> SendDataToDeviceAsync<T>(Guid deviceId, string eventName, T data)
        {
            var connectionId = _connectionMapping.GetConnections(deviceId).FirstOrDefault();
            if (connectionId == null)
            {
                _logger.LogWarning("No connection found for device {DeviceId}", deviceId);
                return HttpStatusCode.NotFound;
            }

            var encryptedData = AesService.EncryptStringToBytes_Aes(JsonConvert.SerializeObject(data));
            await Clients.Client(connectionId).SendAsync(eventName, Convert.ToBase64String(encryptedData));
            _logger.LogInformation("Sending {EventName} with connection {DeviceId}", deviceId, connectionId);
            return HttpStatusCode.OK;
        }
        [AllowAnonymous]
        public async Task TestClose()
        {
            Guid Id = new Guid("6C1D08D1-4BAC-44DA-BDBA-3165799C0497");
            await SendCommandToEsp32(Id, new CommandDto { Command = CommandEnum.Close });
        }
        [AllowAnonymous]
        public async Task TestSettings()
        {
            Guid Id = new Guid("6C1D08D1-4BAC-44DA-BDBA-3165799C0497");
            await SetSettingsOnEsp32(Id, new UserSettings { IsProtected = true });
        }
        private T _decryptRequest<T>(string cryptedBase64)
        {
            byte[] sensorDataByte = Convert.FromBase64String(cryptedBase64);
            string jsonData = AesService.DecryptStringFromBytes_Aes(sensorDataByte);
            return JsonConvert.DeserializeObject<T>(jsonData) ?? throw new AppException("Invalid deserialize object", HttpStatusCode.BadRequest);
        }
    }
}
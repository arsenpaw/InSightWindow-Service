using FirebaseAdmin.Messaging;
using InSightWindowAPI.Models.Sensors;
using InSightWindowAPI.SensorDataProcessors.Interfaces;
using InSightWindowAPI.Serivces.Interfaces;

namespace InSightWindowAPI.SensorDataProcessors
{
    public class AlarmDataProcessor: ISensorDataProcessor<AlarmSensor>
    {
        private readonly IPushNotificationService _pushNotificationService;
        
        public AlarmDataProcessor(IPushNotificationService pushNotificationService)
        {
            _pushNotificationService = pushNotificationService;
        }
        public async Task ProcessDataAsync(AlarmSensor data, Guid userId, Guid deviceId)
        {
            if (!data.IsAlarm)
            {
                return;
            }
            //TODO Latter move to database
            var message = new Message()
            {
                Notification = new FirebaseAdmin.Messaging.Notification()
                {
                    Title = "Warning",
                    Body = "Your device has reposted alarm"
                }
            };
            
            await _pushNotificationService.SendNotificationToUser(userId, message);
        }
    }
}
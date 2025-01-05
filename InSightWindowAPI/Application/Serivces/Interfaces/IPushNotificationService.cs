using FirebaseAdmin.Messaging;

namespace InSightWindowAPI.Serivces.Interfaces
{
    public interface IPushNotificationService
    {
        Task SendNotificationToUser(Guid userId, Message message);


    }
}

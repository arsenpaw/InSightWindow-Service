namespace InSightWindowAPI.Serivces.Interfaces
{
    public interface IPushNotificationService
    {
        Task SendNotificationToUser(Guid userId, string title, string body);


    }
}

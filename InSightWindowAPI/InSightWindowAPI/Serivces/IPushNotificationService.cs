namespace InSightWindowAPI.Serivces
{
    public interface IPushNotificationService
    {
        Task SendNotificationToUser(Guid userId);


    }
}

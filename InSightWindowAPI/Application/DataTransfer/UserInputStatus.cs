namespace InSightWindowAPI.Models.Dto
{
    public class UserInputStatus
    {
        public Guid DeviceId { get; set; }  

        public bool IsProtectedButton { get; set; }

        public bool IsOpenButton { get; set; }

    }
}

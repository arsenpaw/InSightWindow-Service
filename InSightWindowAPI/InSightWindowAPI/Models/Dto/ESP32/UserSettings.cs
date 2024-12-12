namespace InSightWindowAPI.Models.Dto.ESP32
{
    public class UserSettings
    {
        public bool IsProtected { get; set; }

        public AutoBehaviourSettings AutoBehaviourSettings { get; set; } = new AutoBehaviourSettings();
    }
}

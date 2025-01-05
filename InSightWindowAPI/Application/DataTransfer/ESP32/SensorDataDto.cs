namespace InSightWindowAPI.Models.Dto.ESP32
{
    public class SensorDataDto
    {
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public bool IsRain { get; set; }
        public bool IsOpen { get; set; }

        public bool IsAlarm { get; set; }
    }
}

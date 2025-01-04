using Domain.Entity.DeviceModel;

namespace InSightWindowAPI.Models.DeviceModel
{
    public class Window : Device
    {

        public bool IsOpen { get; set; }

        public bool isAlarm { get; set; }

        public bool IsProtected { get; set; }


    }
}

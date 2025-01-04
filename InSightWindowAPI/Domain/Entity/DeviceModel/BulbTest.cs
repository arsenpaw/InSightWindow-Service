using Domain.Entity.DeviceModel;

namespace InSightWindowAPI.Models.DeviceModel
{
    public class BulbTest: Device
    {
        public bool isOn {  get; set; }    
        
        public int LightPowered { get; set; }   
    }
}

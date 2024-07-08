using InSightWindowAPI.Models.DeviceModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InSightWindowAPI.Models.Dto
{
    public class AllWindowDataDto: Window
    {
        public int Temparature { get; set; }

        public int Humidity { get; set; }

        public int isRain { get; set; }

    }
}

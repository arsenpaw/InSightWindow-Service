using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace InSightWindowAPI
{
    public class WindowStatus
    {
        public int Temparature { get; set; }

        public int Humidity { get; set; }

        public string IsOpen { get; set; }

        
        public int isRain { get; set; }

        public string IsProtected { get; set; }

    }
  
}

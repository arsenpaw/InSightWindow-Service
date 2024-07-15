using NuGet.Protocol;
using InSightWindowAPI.Models.DeviceModel;
using System.Collections;
namespace InSightWindowAPI
{
    public class DeviceType 
    {
        public List<string> AllowedDevice { get; init; } = new List<string>();
        public DeviceType()
        {
            AllowedDevice.Add(nameof(Window));
            AllowedDevice.Add(nameof(BulbTest));
        }
        public override bool Equals(object? obj)
        {
            if (obj is string str)
            {
                return AllowedDevice.Contains(str);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AllowedDevice);
        }
    }
}

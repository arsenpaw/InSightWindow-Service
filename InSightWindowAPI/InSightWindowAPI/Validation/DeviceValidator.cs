using FluentValidation;
using InSightWindow.Models;
using InSightWindowAPI.Models.DeviceModel;

namespace InSightWindowAPI.Validation
{
    public class DeviceDtoValidator : FluentValidation.AbstractValidator<DeviceDto>
    {
        public DeviceDtoValidator()
        {
            RuleFor(x => x.DeviceType)
            .NotEmpty()
            .Must(deviceType => new DeviceType().AllowedDevice.Contains(deviceType))
            .WithMessage("DeviceType must be one of the allowed devices.");

        }
    }
    public class DeviceValidator : FluentValidation.AbstractValidator<Device>
    {
        public DeviceValidator()
        {
            RuleFor(x => x.DeviceType)
            .NotEmpty()
            .Must(deviceType => new DeviceType().AllowedDevice.Contains(deviceType))
            .WithMessage("DeviceType must be one of the allowed devices.");

        }
    }
    public class DeviceType
    {
        public List<string> AllowedDevice { get; init; } = new List<string>();
        public DeviceType()
        {
            AllowedDevice.Add(nameof(Window));
            AllowedDevice.Add(nameof(BulbTest));
            AllowedDevice.Add(nameof(Device));
        }

        public override bool Equals(object? obj)
        {
            if (obj is string deviceName)
            {
                return AllowedDevice.Contains(deviceName);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(AllowedDevice);
        }
    }
}

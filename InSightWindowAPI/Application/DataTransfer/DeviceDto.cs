using System.ComponentModel.DataAnnotations;

namespace InSightWindow.Models
{
    public record DeviceDto
    {
        [Required]
        public Guid Id { get; private set; }  

        [Required]
        public string DeviceType { get; set; }
    }
}

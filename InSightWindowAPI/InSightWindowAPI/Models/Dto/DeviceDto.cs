using System.ComponentModel.DataAnnotations;

namespace InSightWindowAPI.Models.Dto
{
    public record DeviceDto
    {
        [Required]
        public string DeviceType { get; set; }
    }
}

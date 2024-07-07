using System.ComponentModel.DataAnnotations;

namespace InSightWindowAPI.Models.Dto
{
    public record DeviceDto
    {
        
        public Guid? Id { get; private set; }  

        [Required]
        public string DeviceType { get; set; }
    }
}

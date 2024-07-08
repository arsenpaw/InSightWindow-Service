using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; 
using System.Reflection.Metadata;
using InSightWindowAPI.Models.Dto;

namespace InSightWindowAPI.Models.DeviceModel
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get;  set; } = Guid.NewGuid();

        public required string DeviceType { get; set; } 

        public virtual Guid? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
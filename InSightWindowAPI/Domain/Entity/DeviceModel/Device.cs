using InSightWindowAPI.Models.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity.DeviceModel
{
    public  class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get;  set; } = Guid.NewGuid();

        public required string DeviceType { get; set; } 

        public  Guid? UserId { get; set; }

        public  User? User { get; set; }
    }
}
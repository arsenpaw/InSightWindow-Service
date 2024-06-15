using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using InSightWindowAPI.Models.Dto;

namespace InSightWindowAPI.Models
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        [Required]
        public string Name { get; set; } = null!;

        public Guid? UserId { get; set; }

        public User? User { get; set; }
    }
}
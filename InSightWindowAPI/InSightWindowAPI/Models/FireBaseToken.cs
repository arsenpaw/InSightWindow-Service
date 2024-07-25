using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InSightWindowAPI.Models
{
    public class FireBaseToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        public Guid UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public string Token { get; set; }
    }
}

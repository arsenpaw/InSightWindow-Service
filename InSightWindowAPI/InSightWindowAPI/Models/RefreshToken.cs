using System.ComponentModel.DataAnnotations;

namespace InSightWindowAPI.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }    

        [Required]
        public string Token { get; set; }   

        public  DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime ExpitedDate {  get; set; }
        
        public Guid UserId { get; set; }    

        public User User { get; set; }
    }
}

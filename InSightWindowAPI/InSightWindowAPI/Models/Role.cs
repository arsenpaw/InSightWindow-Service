using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InSightWindowAPI.Enums;

namespace InSightWindowAPI.Models
{
    public class Role
    {
        [Key]
        public Guid Id {  get; set; }

        public string RoleName { get; set; } = UserRole.USER;

        
        public Guid UserId { get; set; }

       
        public User User { get; set; } 


    }
}

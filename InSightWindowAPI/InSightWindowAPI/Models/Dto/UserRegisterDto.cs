using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InSightWindowAPI.Models.Dto
{
    public record UserRegisterDto : UserLoginDto
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

    }
}

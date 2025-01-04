namespace InSightWindowAPI.Models.Dto
{
    using System.ComponentModel.DataAnnotations;
        public class UserRegisterDto
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }
        }
    

}

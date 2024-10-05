﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InSightWindowAPI.Models
{
    public class FireBaseToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }

        [Required]
        public string Token { get; set; }

        public ICollection<User> Users { get; }

        public ICollection<UserFireBaseTokens> UserFireBaseTokens { get; }
    }
}

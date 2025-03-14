﻿using System.ComponentModel.DataAnnotations;

namespace InSightWindowAPI.Models.Dto
{
    public class RefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}

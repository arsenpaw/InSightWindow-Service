﻿using Domain.Entity.DeviceModel;
using InSightWindowAPI.Models.DeviceModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InSightWindowAPI.Models.Entity
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : IdentityUser<Guid>
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public ICollection<FireBaseToken> FireBaseTokens { get; set; } = new List<FireBaseToken>();

        public ICollection<UserFireBaseTokens> UserFireBaseTokens { get; set; } = new List<UserFireBaseTokens>();

        public ICollection<Device> Devices { get; set; } = new List<Device>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}

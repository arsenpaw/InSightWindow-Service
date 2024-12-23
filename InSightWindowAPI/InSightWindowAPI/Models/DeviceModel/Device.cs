﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using InSightWindowAPI.Models.Dto;
using InSightWindowAPI.Models.Entity;

namespace InSightWindowAPI.Models.DeviceModel
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
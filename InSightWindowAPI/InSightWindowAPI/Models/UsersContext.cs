﻿using NuGet.Protocol;
using InSightWindowAPI.Models.DeviceModel;
using System.Collections;
using InSightWindowAPI.Models.DeviceModel;
using Microsoft.EntityFrameworkCore;
namespace InSightWindowAPI.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Window> WindowDevices { get; set; }
        public DbSet<BulbTest> BulbTestDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
             .HasMany(u => u.Devices)
             .WithOne(d => d.User)
             .HasForeignKey(d => d.UserId);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithOne(x => x.User)
                .HasForeignKey<Role>(x => x.UserId);
            modelBuilder.Entity<User>()
                .HasOne(u => u.RefreshToken)
                .WithOne(x => x.User)
                .HasForeignKey<RefreshToken>(x => x.UserId);
            modelBuilder.Entity<Device>()
                .UseTphMappingStrategy()
                .HasDiscriminator<string>("DeviceType"); 


        }
        public class DeviceType
        {
            public List<string> AllowedDevice { get; init; } = new List<string>();
            public DeviceType()
            {
                AllowedDevice.Add(nameof(Window));
                AllowedDevice.Add(nameof(BulbTest));
                AllowedDevice.Add(nameof(Device));
            }
        }
    }
}



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
        public DbSet<FireBaseToken> FireBaseTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.FireBaseToken)
                .WithOne(f => f.User)
                .HasForeignKey<FireBaseToken>(f => f.UserId);
            modelBuilder.Entity<FireBaseToken>()
               .HasIndex(u => u.UserId)
               .IsUnique();

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
     
    }
}



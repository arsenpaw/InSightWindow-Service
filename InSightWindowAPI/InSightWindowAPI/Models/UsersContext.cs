﻿
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
             .HasMany(u => u.Devices)
             .WithOne(d => d.User)
             .HasForeignKey(d => d.UserId);


        }

    }
}

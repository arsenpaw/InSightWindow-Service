using Domain.Entity.DeviceModel;
using InSightWindowAPI.Models.DeviceModel;
using InSightWindowAPI.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class UsersContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Window> WindowDevices { get; set; }
        public DbSet<BulbTest> BulbTestDevices { get; set; }
        public DbSet<FireBaseToken> FireBaseTokens { get; set; }
        public DbSet<UserFireBaseTokens> UserFireBaseTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<RefreshToken>()
             .HasOne(rt => rt.User)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FireBaseTokens)
                .WithMany(e => e.Users)
                .UsingEntity<UserFireBaseTokens>();

            modelBuilder.Entity<User>()
                .HasMany(e => e.Devices)
                .WithOne(e => e.User);

            modelBuilder.Entity<Device>()
                .UseTphMappingStrategy()
                .HasDiscriminator<string>("DeviceType");
        }
    }
}

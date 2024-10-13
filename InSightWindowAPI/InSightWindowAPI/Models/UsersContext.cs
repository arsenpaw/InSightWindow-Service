using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI.Models.DeviceModel;
using Microsoft.AspNetCore.Identity;

namespace InSightWindowAPI.Models
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

            base.OnModelCreating(modelBuilder); // Ensure Identity-related tables are set up

           

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

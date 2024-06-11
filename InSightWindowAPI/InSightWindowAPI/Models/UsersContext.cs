using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace InSightWindowAPI.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options)
        {

        }
        public DbSet<UserRegister> Users { get; set; }
        public DbSet<Device> Devices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRegister>()
             .HasMany(u => u.Devices)
             .WithOne(d => d.User)
             .HasForeignKey(d => d.UserId);


        }

    }
}

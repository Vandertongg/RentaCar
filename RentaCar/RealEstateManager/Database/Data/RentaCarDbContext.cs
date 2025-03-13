using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RentaCar.RealEstateManager.Database.Data.Entities;
using System.Reflection.Emit;

namespace RentaCar.RealEstateManager.Database.Data
{
    public class RentaCarDbContext : IdentityDbContext<User>
    {
        public RentaCarDbContext(DbContextOptions<RentaCarDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Booking>()
                .HasKey(ucb => new { ucb.UserId, ucb.CarId });

            // Booking има собствен първичен ключ Id
            builder.Entity<Booking>()
                .HasKey(b => b.Pk);

            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Booking>()
                .HasOne(b => b.Car)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Уникални индекси за User:
            builder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.IdentificationNumber)
                .IsUnique();
            builder.Entity<Booking>()
                .Property(o => o.StartDate)
                .HasColumnType("DATETIME2");

            builder.Entity<Booking>()
                .Property(o => o.EndDate)
                .HasColumnType("DATETIME2");
            builder.Entity<Car>()
                .Property(c => c.PricePerDay)
                .HasColumnType("decimal(18,2)");

        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<RentNews> RentNews { get; set; }
    }
}

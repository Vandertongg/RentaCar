using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.Database.Data.Entities
{
    [Table("Booking", Schema = "blg")]
    public class Booking
    {
        [Key]
        public int Pk { get; set; }

        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public User User { get; set; } = null!;

        [ForeignKey(nameof(Car))]
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;
        public bool IsApproved { get; set; } = false;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
    }

    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Canceled,
        Completed
    }
}

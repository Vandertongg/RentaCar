using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.Database.Models
{
    [Table("Booking", Schema = "blg")]
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

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

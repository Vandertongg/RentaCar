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
        [Required]
        public string? UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey(nameof(Car))]
        [Required]
        public int CarId { get; set; }
        public Car? Car { get; set; }

        public bool IsApproved { get; set; } = false;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        [BookingDateValidation]
        public DateTime EndDate { get; set; }

        [Required]
        [BookingDateValidation]
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
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentaCar.RealEstateManager.Database.Models
{
    [Table("Car", Schema = "blg")]
    public class Car
    {
        [PrimaryKey]
        public int Pk { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public int PassangerSeats { get; set; } 
        public string? Description { get; set; }
        public  string? Picture { get; set; }
        public bool IsAvailable { get; set; } = true;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class CarViewModel
    {
      
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? Picture { get; set; }
        public ICollection<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();
    }
}

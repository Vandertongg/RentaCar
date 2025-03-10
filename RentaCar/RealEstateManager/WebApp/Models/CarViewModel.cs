using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class CarViewModel
    {

        public int Pk { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public int PassangerSeats { get; set; }
        public string? Description { get; set; }
        public string? Picture { get; set; }
    }
}

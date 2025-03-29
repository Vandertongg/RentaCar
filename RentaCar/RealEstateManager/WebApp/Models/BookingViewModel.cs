using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class BookingViewModel
    {
      
        public int Pk { get; set; }
        public int UserId { get; set; }
       
        public string UserName { get; set; } = string.Empty;
        public int CarId { get; set; }
        
        public string CarBrand { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending";

        public int Duration => (EndDate - StartDate).Days;
    }
}

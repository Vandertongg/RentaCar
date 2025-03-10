using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class BookingViewModel
    {
      
        public int Id { get; set; }
        public string UserId { get; set; }
       
        public UserViewModel User { get; set; }
        public int CarId { get; set; }
        
        public CarViewModel Car { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending";
    }
}

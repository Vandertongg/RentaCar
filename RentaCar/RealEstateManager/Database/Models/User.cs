using Microsoft.AspNetCore.Identity;

namespace RentaCar.RealEstateManager.Database.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public ICollection<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();
    }
}

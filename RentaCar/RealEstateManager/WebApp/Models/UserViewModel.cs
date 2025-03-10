namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class UserViewModel
    {
        public string FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public ICollection<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();
    }
}

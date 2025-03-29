using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class UserViewModel
    {
        
        public int Pk { get; set; }
        public string Password { get; set; } = null!;
        public string IdentificationNumber { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public  string? UserName { get; set; }
        public  string? Email { get; set; }
        public uint Age { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}

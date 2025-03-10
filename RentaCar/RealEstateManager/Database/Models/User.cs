using Microsoft.AspNetCore.Identity;
using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentaCar.RealEstateManager.Database.Models
{
    [Table("User", Schema = "blg")]
    public class User : IdentityUser
    {
        [PrimaryKey]
        public int Pk { get; set; }
        [Unique]
        public string? IdentificationNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Unique]
        public  override string?  UserName { get; set; }

        [Unique]
        public override string? Email { get; set; }

        public uint Age { get; set; }
        public string? ProfilePicture { get; set; }
        public ICollection<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();
    }
}

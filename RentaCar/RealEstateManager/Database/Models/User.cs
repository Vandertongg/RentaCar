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
        [Required]
        public string IdentificationNumber { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Unique]
        [Required]
        public  override string  UserName { get; set; } = null!;

        [Unique]
        [Required]
        public override string? Email { get; set; } = null!;

        [Required]
         [System.ComponentModel.DataAnnotations.Range(18, int.MinValue, ErrorMessage="You must be at least 18 years old to be permitted to book!")]
        public uint Age { get; set; }
        public string? ProfilePicture { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentaCar.RealEstateManager.Database.Data.Entities
{
    [Table("User", Schema = "blg")]
    public class User : IdentityUser
    {
        [Key]
        public int Pk { get; set; }

        [Required]
        public string IdentificationNumber { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        public override string UserName { get; set; } = null!;

        [Required]
        public override string? Email { get; set; } = null!;

        [Required]
        [Range(18, int.MinValue, ErrorMessage = "You must be at least 18 years old to be permitted to book!")]
        public uint Age { get; set; }
        public string? ProfilePicture { get; set; }

        // Навигационно свойство за връзката много към много чрез Booking
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}

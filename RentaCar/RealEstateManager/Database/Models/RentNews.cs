using ServiceStack.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentaCar.RealEstateManager.Database.Models
{
    [Table("RentNews", Schema = "blg")]
    public class RentNews
    {
        [PrimaryKey]
        public int Pk { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Content { get; set; } = null!;
        public string? NewsPicture { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}

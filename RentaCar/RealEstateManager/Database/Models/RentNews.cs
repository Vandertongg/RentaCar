using System.ComponentModel.DataAnnotations;

namespace RentaCar.RealEstateManager.Database.Models
{
    public class RentNews
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? NewsPicture { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}

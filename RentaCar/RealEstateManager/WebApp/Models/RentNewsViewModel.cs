using Microsoft.AspNetCore.Http;

namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class RentNewsViewModel
    {
        public int Pk { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public IFormFile? NewsPicture { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}

namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class RentNewsViewModel
    {
        public int Pk { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? NewsPicture { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}

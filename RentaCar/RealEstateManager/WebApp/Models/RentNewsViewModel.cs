namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class RentNewsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? NewsPicture { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}

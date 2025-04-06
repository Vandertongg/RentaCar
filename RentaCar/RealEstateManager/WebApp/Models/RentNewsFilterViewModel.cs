using RentaCar.RealEstateManager.Database.Data.Entities;

namespace RentaCar.RealEstateManager.WebApp.Models
{
    internal class RentNewsFilterViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public PaginatedList<RentNews> RentNews { get; set; }
    }
}
using RentaCar.RealEstateManager.Database.Data.Entities;

namespace RentaCar.RealEstateManager.WebApp.Models
{
    public class CarSearchViewModel
    {
        public string? SearchTerm { get; set; }
        public string? Brand { get; set; }
        public int? Year { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinSeats { get; set; }
        public bool? IsAvailable { get; set; }
        public string? SortBy { get; set; }
        public List<Car> Cars { get; set; } = new List<Car>();

        
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;

            
        }

    }


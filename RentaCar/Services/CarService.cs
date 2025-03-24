using RentaCar.RealEstateManager.Database.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RentaCar.Services
{
    public class CarService
    {
        private readonly ICar _carRepository;

        public CarService(ICar carRepository)
        {
            _carRepository = carRepository;
        }

        // Метод за сортиране на коли по зададен критерий
        public List<Car> SortCars(string orderBy, bool isAscending)
        {
            var cars = _carRepository.GetAllCars();

            if (isAscending)
            {
                switch (orderBy.ToLower())
                {
                    case "brand":
                        return cars.OrderBy(c => c.Brand).ToList();
                    case "model":
                        return cars.OrderBy(c => c.Model).ToList();
                    case "year":
                        return cars.OrderBy(c => c.Year).ToList();
                    case "priceperday":
                        return cars.OrderBy(c => c.PricePerDay).ToList();
                    default:
                        throw new ArgumentException("Невалиден критерий за сортиране.");
                }
            }
            else
            {
                switch (orderBy.ToLower())
                {
                    case "brand":
                        return cars.OrderByDescending(c => c.Brand).ToList();
                    case "model":
                        return cars.OrderByDescending(c => c.Model).ToList();
                    case "year":
                        return cars.OrderByDescending(c => c.Year).ToList();
                    case "priceperday":
                        return cars.OrderByDescending(c => c.PricePerDay).ToList();
                    default:
                        throw new ArgumentException("Невалиден критерий за сортиране.");
                }
            }
        }

        // Метод за филтриране на коли по зададен критерий
        public List<Car> FilterCars(string? brand = null, string? model = null, int? year = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var cars = _carRepository.GetAllCars();

            if (brand != null)
                cars = cars.Where(c => c.Brand?.ToLower() == brand.ToLower()).ToList();

            if (model != null)
                cars = cars.Where(c => c.Model?.ToLower() == model.ToLower()).ToList();

            if (year.HasValue)
                cars = cars.Where(c => c.Year == year.Value).ToList();

            if (minPrice.HasValue)
                cars = cars.Where(c => c.PricePerDay >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                cars = cars.Where(c => c.PricePerDay <= maxPrice.Value).ToList();

            return cars;
        }
    }
}

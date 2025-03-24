using RentaCar.RealEstateManager.Database.Data.Entities;

namespace RentaCar.Services
{
    public interface ICar
    {
        List<Car> GetAllCars();
        Car GetCarById(int id);
        void AddCar(Car car);
        void UpdateCar(Car car);
        void DeleteCar(int id);
    }
}

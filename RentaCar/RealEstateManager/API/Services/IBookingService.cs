namespace RentaCar.RealEstateManager.API.Services
{
    public interface IBookingService
    {
        Task<bool> ApproveBookingAsync(int bookingId, string userRole);
        Task<bool> RejectBookingAsync(int bookingId, string userRole, string? rejectionReason = null);

        // Добавяне на метода за валидация на дати
        (bool isValid, string message) ValidateDates(DateTime startDate, DateTime endDate);
    }

}

using RentaCar.RealEstateManager.API.Services;
using RentaCar.RealEstateManager.Database.Data;
using RentaCar.RealEstateManager.Database.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class BookingService : IBookingService
{
    private readonly RentaCarDbContext _context;

    public BookingService(RentaCarDbContext context)
    {
        _context = context;
    }

    // Одобрение на заявка
    public async Task<bool> ApproveBookingAsync(int bookingId, string userRole)
    {
        if (!IsAuthorizedToApproveOrReject(userRole))
        {
            return false; // Нямате право да одобрявате заявки
        }

        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null)
        {
            return false; // Заявката не е намерена
        }

        booking.IsApproved = true;
        booking.Status = BookingStatus.Confirmed;

        await _context.SaveChangesAsync();
        return true;
    }

    // Отказване на заявка
    public async Task<bool> RejectBookingAsync(int bookingId, string userRole, string? rejectionReason = null)
    {
        if (!IsAuthorizedToApproveOrReject(userRole))
        {
            return false; // Нямате право да отказвате заявки
        }

        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null)
        {
            return false; // Заявката не е намерена
        }

        booking.IsApproved = false;
        booking.Status = BookingStatus.Canceled;

        // Ако искате да запазите причината за отказа, добавете поле в модела Booking
        // booking.RejectionReason = rejectionReason;

        await _context.SaveChangesAsync();
        return true;
    }

    // Валидация на StartDate и EndDate
    public (bool isValid, string message) ValidateDates(DateTime startDate, DateTime endDate)
    {
        var currentDate = DateTime.Now.Date;

        // Проверка дали StartDate е в миналото
        if (startDate.Date < currentDate)
        {
            return (false, "Start date cannot be in the past.");
        }

        // Проверка дали EndDate е преди StartDate
        if (endDate.Date < startDate.Date)
        {
            return (false, "End date cannot be before start date.");
        }

        return (true, "Dates are valid.");
    }

    // Проверка дали потребителят има право да одобрява или отказва заявки
    private bool IsAuthorizedToApproveOrReject(string userRole)
    {
        return userRole == "Admin" || userRole == "MehanicSpecialized";
    }
}
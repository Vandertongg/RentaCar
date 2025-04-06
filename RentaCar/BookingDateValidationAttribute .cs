using RentaCar.RealEstateManager.Database.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;

public class BookingDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var booking = (Booking)validationContext.ObjectInstance;

        if (booking.StartDate.Date < DateTime.Now.Date)
        {
            return new ValidationResult("Start date cannot be in the past.");
        }

        if (booking.EndDate.Date < booking.StartDate.Date)
        {
            return new ValidationResult("End date cannot be before start date.");
        }

        return ValidationResult.Success;
    }
}
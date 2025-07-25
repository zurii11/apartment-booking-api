using ApartmentBooking.Models;

namespace ApartmentBooking.Services;

public interface IApartmentService
{
    Task<List<ApartmentDto>> GetAvailableApartmentsAsync(DateTime start_date, DateTime end_date);
    Task<bool?> IsAvailableAsync(int id, DateTime start_date, DateTime end_date);
}

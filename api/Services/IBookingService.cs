namespace ApartmentBooking.Services;

public interface IBookingService
{
    Task<bool> CreateBookingAsync(int apartment_id, int? user_id, DateTime start_date, DateTime end_date);
    Task<bool> DeleteBookingAsync(int id);
}

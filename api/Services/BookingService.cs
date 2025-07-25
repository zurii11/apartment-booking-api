using System.Data;
using Microsoft.Data.SqlClient;

namespace ApartmentBooking.Services;

public class BookingService : IBookingService
{
    private readonly IConfiguration _config;

    public BookingService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> CreateBookingAsync(int apartment_id, int? user_id, DateTime start_date, DateTime end_date)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        using var create_booking_procedure = new SqlCommand("CreateBooking", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        create_booking_procedure.Parameters.AddWithValue("@ApartmentId", apartment_id);
        create_booking_procedure.Parameters.AddWithValue("@UserId", user_id);
        create_booking_procedure.Parameters.AddWithValue("@StartDate", start_date);
        create_booking_procedure.Parameters.AddWithValue("@EndDate", end_date);

        try
        {
            await create_booking_procedure.ExecuteNonQueryAsync();
            return true;
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Failed creating a booking with ERROR: {ex.Number}");
            Console.WriteLine($"ApartmentId: {apartment_id}, UserId: {user_id}, StartDate: {start_date}, EndDate: {end_date}");
            return false;
        }
    }

    public async Task<bool> DeleteBookingAsync(int id)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        using var delete_booking_procedure = new SqlCommand("DeleteBooking", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        delete_booking_procedure.Parameters.AddWithValue("@BookingId", id);

        try
        {
            await delete_booking_procedure.ExecuteNonQueryAsync();
            return true;
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Failed deleting a booking with ERROR: {ex.Number}");
            return false;
        }
    }
}

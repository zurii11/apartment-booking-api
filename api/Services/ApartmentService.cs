using System.Data;
using Microsoft.Data.SqlClient;

namespace ApartmentBooking.Services;

public class ApartmentService : IApartmentService
{
    private readonly IConfiguration _config;

    public ApartmentService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<List<ApartmentDto>> GetAvailableApartmentsAsync(DateTime start_date, DateTime end_date)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        using var get_apartments_procedure = new SqlCommand("ListAvailableApartments", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        get_apartments_procedure.Parameters.AddWithValue("@StartDate", start_date);
        get_apartments_procedure.Parameters.AddWithValue("@EndDate", end_date);

        using var reader = await get_apartments_procedure.ExecuteReaderAsync();

        List<ApartmentDto> apartments = new List<ApartmentDto>();

        while (await reader.ReadAsync())
        {
            apartments.Add(new ApartmentDto
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                PricePerNight = reader.GetDecimal(3)
            });
        }

        return apartments;
    }

    public async Task<bool?> IsAvailableAsync(int id, DateTime start_date, DateTime end_date)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        using var get_apartments_procedure = new SqlCommand("CheckApartmentAvailability", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        get_apartments_procedure.Parameters.AddWithValue("@ApartmentId", id);
        get_apartments_procedure.Parameters.AddWithValue("@StartDate", start_date);
        get_apartments_procedure.Parameters.AddWithValue("@EndDate", end_date);

        using var reader = await get_apartments_procedure.ExecuteReaderAsync();

        if(await reader.ReadAsync())
            return reader.GetBoolean(0);

        return null;
    }
}

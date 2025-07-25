using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using ApartmentBooking.Helpers;

namespace ApartmentBooking.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IJwtGenerator _jwtGenerator;

    public AuthService(IConfiguration config, IJwtGenerator jwtGenerator)
    {
        _config = config;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<bool> RegisterAsync(string username, string password)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        string hashed_password = HashPassword(password);

        using var register_procedure_cmd = new SqlCommand("RegisterUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        register_procedure_cmd.Parameters.AddWithValue("@Username", username);
        register_procedure_cmd.Parameters.AddWithValue("@PasswordHash", hashed_password);

        try
        {
            await register_procedure_cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Couldn't register the user. ERROR NUMBER {ex.Number}");
            return false;
        }
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        using var get_user_cmd = new SqlCommand("GetUserByUsername", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        get_user_cmd.Parameters.AddWithValue("@Username", username);

        using var reader = await get_user_cmd.ExecuteReaderAsync();
        if (!reader.Read()) return null;

        int user_id = reader.GetInt32(0);
        string stored_hash = reader.GetString(1);

        if (!VerifyPassword(password, stored_hash))
            return null;

        return _jwtGenerator.GenerateToken(user_id, username);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string stored_hash)
    {
        var hash_of_input = HashPassword(password);
        return hash_of_input == stored_hash;
    }
}

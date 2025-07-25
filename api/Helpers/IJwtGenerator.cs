namespace ApartmentBooking.Helpers;

public interface IJwtGenerator
{
    string GenerateToken(int user_id, string username);
}

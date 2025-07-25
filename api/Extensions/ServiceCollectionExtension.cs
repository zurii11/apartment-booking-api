using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ApartmentBooking.Helpers;
using ApartmentBooking.Services;

namespace ApartmentBooking.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IApartmentService, ApartmentService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddSingleton<IJwtGenerator, JwtGenerator>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]))
                };
            });

        return services;
    }
}

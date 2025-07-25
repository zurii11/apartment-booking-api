using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ApartmentBooking.Services;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService booking_service)
    {
        _bookingService = booking_service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromQuery] int apartment_id, [FromQuery] DateTime start_date, [FromQuery] DateTime end_date)
    {
        if(start_date >= end_date)
            return BadRequest("Start date must be before end date");

        var user_id_claim = User.FindFirst(ClaimTypes.NameIdentifier);
        int? user_id = null;

        if(user_id_claim != null && int.TryParse(user_id_claim.Value, out var parsed_id))
            user_id = parsed_id;


        var result = await _bookingService.CreateBookingAsync(apartment_id, user_id, start_date, end_date);

        if(!result)
            return BadRequest("Could not create booking");

        return Ok("Booking created.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var result = await _bookingService.DeleteBookingAsync(id);

        if(!result)
            return NotFound("Booking not found");

        return Ok("Booking deleted");
    }
}

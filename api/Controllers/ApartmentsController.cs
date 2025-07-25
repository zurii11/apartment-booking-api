using Microsoft.AspNetCore.Mvc;
using ApartmentBooking.Services;
using ApartmentBooking.Models;

[ApiController]
[Route("api/[controller]")]
public class ApartmentsController : ControllerBase
{
    private readonly IApartmentService _apartmentService;

    public ApartmentsController(IApartmentService apt_service)
    {
        _apartmentService = apt_service;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableApartments([FromQuery] DateTime start_date, [FromQuery] DateTime end_date)
    {
        if(start_date >= end_date)
            return BadRequest("Start date must be before end date");

        List<ApartmentDto> apartments = await _apartmentService.GetAvailableApartmentsAsync(start_date, end_date);
        return Ok(apartments);
    }

    [HttpGet("{id}/availability")]
    public async Task<IActionResult> CheckApartmentAvailability(int id, [FromQuery] DateTime start_date, [FromQuery] DateTime end_date)
    {
        if(start_date >= end_date)
            return BadRequest("Start date must be before end date");

        bool? is_available = await _apartmentService.IsAvailableAsync(id, start_date, end_date);

        if(is_available is null)
            return NotFound();

        return Ok(new { Available = is_available });
    }

}


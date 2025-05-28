using Microsoft.AspNetCore.Mvc;
using RestaurantReservationSystem.Facades;
using RestaurantReservationSystem.Models;
using RestaurantReservationSystem.Builders;
using RestaurantReservationSystem.Dtos;
using RestaurantReservationSystem.Strategies;
using RestaurantReservationSystem.Managers;

namespace RestaurantReservationSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var reservations = ReservationManager.Instance.GetAll();
        return Ok(reservations);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReservationDto dto)
    {
        var builder = new ReservationBuilder()
            .SetGuestName(dto.GuestName)
            .SetDateTime(dto.DateTime)
            .SetNumberOfGuests(dto.NumberOfGuests);

        if (!string.IsNullOrWhiteSpace(dto.SpecialRequest))
            builder.SetDetails(dto.SpecialRequest);

        var reservation = builder.Build();

        var success = await new ReservationFacade()
            .CreateReservationAsync(reservation, new OnlineReservationStrategy());

        if (!success) return BadRequest("Geen beschikbare tafel");

        return Ok(reservation);
    }
}
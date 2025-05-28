namespace RestaurantReservationSystem.Dtos;

public class ReservationDto
{
    public string GuestName { get; set; }
    public DateTime DateTime { get; set; }
    public int NumberOfGuests { get; set; }
    public string? SpecialRequest { get; set; } // optioneel
}
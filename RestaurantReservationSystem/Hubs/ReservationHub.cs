using Microsoft.AspNetCore.SignalR;
using RestaurantReservationSystem.Models;

namespace RestaurantReservationSystem.Hubs;

public class ReservationHub : Hub
{
    public async Task SendConfirmation(Reservation reservation)
    {
        await Clients.All.SendAsync("ReceiveReservationUpdate", reservation.ToString());
    }
}
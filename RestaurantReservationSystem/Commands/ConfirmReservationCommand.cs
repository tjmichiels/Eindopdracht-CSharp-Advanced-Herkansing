using Microsoft.AspNetCore.SignalR;
using RestaurantReservationSystem.Concurrency;
using RestaurantReservationSystem.Enums;
using RestaurantReservationSystem.Hubs;
using RestaurantReservationSystem.Models;
using RestaurantReservationSystem.Observers;
using RestaurantReservationSystem.Services;
using RestaurantReservationSystem.States;

namespace RestaurantReservationSystem.Commands;

public class ConfirmReservationCommand : IReservationCommand
{
    private readonly Reservation _reservation;
    private readonly NotificationService _notificationService;
    private readonly IHubContext<ReservationHub> _hubContext;

    public ConfirmReservationCommand(Reservation reservation)
    {
        _reservation = reservation;
        _notificationService = new NotificationService();
        _notificationService.Subscribe(new EmailNotificationObserver());
    }

    public void Execute()
    {
        _reservation.ReservationState = new ConfirmedReservationState();


        // Concurrency: klantnotificatie + SignalR via thread pool
        ThreadPoolExecutor.QueueTask(async () =>
        {
            await _notificationService.NotifyAllAsync(_reservation);
            await _hubContext.Clients.All.SendAsync("ReceiveReservationUpdate", _reservation.ToString());
        });
    }
}
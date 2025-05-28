using System.Collections.Concurrent;
using RestaurantReservationSystem.Models;

namespace RestaurantReservationSystem.Managers;

public sealed class ReservationManager
{
    private static readonly Lazy<ReservationManager> instance =
        new Lazy<ReservationManager>(() => new ReservationManager());

    // Concurrent Collections
    private readonly ConcurrentDictionary<Guid, Reservation> _reservations
        = new ConcurrentDictionary<Guid, Reservation>();

    // Voor API
    public List<Reservation> GetAll()
    {
        return _reservations.Values.ToList();
    }

    public static ReservationManager Instance => instance.Value;

    private ReservationManager()
    {
    }

    public void AddReservation(Reservation reservation) => _reservations.TryAdd(reservation.Id, reservation);
    public Reservation GetReservation(Guid id) => _reservations[id];
}
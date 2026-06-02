using BookingSystem.Modules.Reservations.Domain;

namespace BookingSystem.Modules.Reservations.Infrastructure;

internal sealed class InMemoryReservationStore
{
    private readonly object _lock = new();
    private readonly List<Reservation> _reservations = [];

    public T Execute<T>(Func<List<Reservation>, T> action)
    {
        lock (_lock)
        {
            return action(_reservations);
        }
    }

    public void Execute(Action<List<Reservation>> action)
    {
        lock (_lock)
        {
            action(_reservations);
        }
    }
}

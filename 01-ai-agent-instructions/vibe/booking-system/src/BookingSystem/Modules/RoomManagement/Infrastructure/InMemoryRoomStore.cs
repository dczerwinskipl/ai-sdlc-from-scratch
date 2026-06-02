using BookingSystem.Modules.RoomManagement.Domain;

namespace BookingSystem.Modules.RoomManagement.Infrastructure;

internal sealed class InMemoryRoomStore
{
    private readonly object _lock = new();
    private readonly List<Room> _rooms = [];

    public T Execute<T>(Func<List<Room>, T> action)
    {
        lock (_lock)
        {
            return action(_rooms);
        }
    }

    public void Execute(Action<List<Room>> action)
    {
        lock (_lock)
        {
            action(_rooms);
        }
    }
}

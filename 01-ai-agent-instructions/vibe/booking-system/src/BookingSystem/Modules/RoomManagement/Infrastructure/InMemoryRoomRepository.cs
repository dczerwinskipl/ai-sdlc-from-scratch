using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

namespace BookingSystem.Modules.RoomManagement.Infrastructure;

internal sealed class InMemoryRoomRepository(InMemoryRoomStore store) : IRoomRepository
{
    public Task<Room?> GetById(RoomId id, CancellationToken cancellationToken)
    {
        var room = store.Execute(rooms => rooms.FirstOrDefault(r => r.Id == id));
        return Task.FromResult(room);
    }

    public Task<IReadOnlyList<Room>> GetAll(CancellationToken cancellationToken)
    {
        var rooms = store.Execute(rooms => (IReadOnlyList<Room>)rooms.ToList());
        return Task.FromResult(rooms);
    }

    public Task Add(Room room, CancellationToken cancellationToken)
    {
        store.Execute(rooms => rooms.Add(room));
        return Task.CompletedTask;
    }

    public Task Update(Room room, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

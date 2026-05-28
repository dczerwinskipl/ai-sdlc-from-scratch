using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.PublicContracts;

namespace BookingSystem.Modules.RoomManagement.Infrastructure;

internal sealed class InMemoryRoomReader(InMemoryRoomStore store) : IRoomReader
{
    public Task<RoomInfo?> GetById(RoomId roomId, CancellationToken cancellationToken)
    {
        var room = store.Execute(rooms => rooms.FirstOrDefault(r => r.Id == roomId));
        if (room is null) return Task.FromResult<RoomInfo?>(null);

        return Task.FromResult<RoomInfo?>(new RoomInfo(
            room.Id,
            room.Name.Value,
            room.Capacity.Value,
            room.Status == RoomStatus.Active));
    }
}

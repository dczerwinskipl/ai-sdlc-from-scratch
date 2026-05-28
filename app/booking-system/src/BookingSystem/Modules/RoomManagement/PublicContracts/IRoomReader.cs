using BookingSystem.Modules.RoomManagement.Domain;

namespace BookingSystem.Modules.RoomManagement.PublicContracts;

public interface IRoomReader
{
    Task<RoomInfo?> GetById(RoomId roomId, CancellationToken cancellationToken);
}

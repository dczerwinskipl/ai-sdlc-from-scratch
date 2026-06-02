namespace BookingSystem.Modules.RoomManagement.PublicContracts;

public interface IRoomReader
{
    Task<RoomInfo?> GetById(Guid roomId, CancellationToken cancellationToken);
}

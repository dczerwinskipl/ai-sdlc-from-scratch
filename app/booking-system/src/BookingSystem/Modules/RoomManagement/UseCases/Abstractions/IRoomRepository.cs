using BookingSystem.Modules.RoomManagement.Domain;

namespace BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

internal interface IRoomRepository
{
    Task<Room?> GetById(RoomId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Room>> GetAll(CancellationToken cancellationToken);
    Task Add(Room room, CancellationToken cancellationToken);
    Task Update(Room room, CancellationToken cancellationToken);
}

using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

namespace BookingSystem.Modules.RoomManagement.UseCases.ListRooms;

internal sealed class ListRoomsHandler(
    IRoomRepository repository) : IQueryHandler<ListRoomsQuery, IReadOnlyList<RoomListItem>>
{
    public async Task<IReadOnlyList<RoomListItem>> Handle(ListRoomsQuery query, CancellationToken cancellationToken)
    {
        var rooms = await repository.GetAll(cancellationToken);
        return rooms
            .Select(r => new RoomListItem(r.Id.Value, r.Name.Value, r.Capacity.Value, r.Status.ToString()))
            .ToList();
    }
}

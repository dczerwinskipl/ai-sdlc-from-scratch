using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

namespace BookingSystem.Modules.RoomManagement.UseCases.GetRoom;

internal sealed class GetRoomHandler(
    IRoomRepository repository) : IQueryHandler<GetRoomQuery, GetRoomResponse?>
{
    public async Task<GetRoomResponse?> Handle(GetRoomQuery query, CancellationToken cancellationToken)
    {
        var room = await repository.GetById(RoomId.From(query.RoomId), cancellationToken);
        if (room is null) return null;

        return new GetRoomResponse(
            room.Id.Value,
            room.Name.Value,
            room.Capacity.Value,
            room.Status.ToString());
    }
}

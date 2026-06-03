using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.RoomManagement.UseCases.ListRooms;

internal sealed record ListRoomsQuery : IQuery<IReadOnlyList<RoomListItem>>;

using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.RoomManagement.UseCases.GetRoom;

internal sealed record GetRoomQuery(Guid RoomId) : IQuery<GetRoomResponse?>;

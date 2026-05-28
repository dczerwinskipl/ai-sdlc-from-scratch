using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.RoomManagement.UseCases.EditRoom;

internal sealed record EditRoomCommand(Guid RoomId, string Name, int Capacity) : ICommand<EditRoomResponse>;

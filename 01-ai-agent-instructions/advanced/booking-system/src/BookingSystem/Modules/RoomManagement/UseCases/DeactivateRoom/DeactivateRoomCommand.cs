using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.RoomManagement.UseCases.DeactivateRoom;

internal sealed record DeactivateRoomCommand(Guid RoomId) : ICommand;

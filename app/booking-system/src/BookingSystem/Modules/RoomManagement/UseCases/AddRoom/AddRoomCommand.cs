using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.RoomManagement.UseCases.AddRoom;

internal sealed record AddRoomCommand(string Name, int Capacity) : ICommand<AddRoomResponse>;

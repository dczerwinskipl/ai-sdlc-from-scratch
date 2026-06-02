namespace BookingSystem.Modules.RoomManagement.UseCases.GetRoom;

internal sealed record GetRoomResponse(Guid RoomId, string Name, int Capacity, string Status);

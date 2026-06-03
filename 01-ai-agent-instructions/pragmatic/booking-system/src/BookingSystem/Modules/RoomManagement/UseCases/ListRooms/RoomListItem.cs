namespace BookingSystem.Modules.RoomManagement.UseCases.ListRooms;

internal sealed record RoomListItem(Guid RoomId, string Name, int Capacity, string Status);

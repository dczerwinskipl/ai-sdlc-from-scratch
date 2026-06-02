namespace BookingSystem.Modules.RoomManagement.PublicContracts;

public sealed record RoomInfo(
    Guid Id,
    string Name,
    int Capacity,
    bool IsActive);

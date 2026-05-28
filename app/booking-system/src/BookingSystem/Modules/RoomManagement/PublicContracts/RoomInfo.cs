using BookingSystem.Modules.RoomManagement.Domain;

namespace BookingSystem.Modules.RoomManagement.PublicContracts;

public sealed record RoomInfo(
    RoomId Id,
    string Name,
    int Capacity,
    bool IsActive);

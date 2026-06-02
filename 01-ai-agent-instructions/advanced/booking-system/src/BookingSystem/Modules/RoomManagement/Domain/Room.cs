using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.RoomManagement.Domain;

internal sealed class Room : AggregateRoot<RoomId>
{
    public RoomName Name { get; private set; } = null!;
    public RoomCapacity Capacity { get; private set; } = null!;
    public RoomStatus Status { get; private set; }

    private Room() { }

    public static Room Create(RoomId id, RoomName name, RoomCapacity capacity)
    {
        var room = new Room { Id = id, Status = RoomStatus.Active };
        room.Name = name;
        room.Capacity = capacity;
        return room;
    }

    public void Rename(RoomName name) => Name = name;

    public void ChangeCapacity(RoomCapacity capacity) => Capacity = capacity;

    public void Deactivate() => Status = RoomStatus.Inactive;

    public bool IsActive => Status == RoomStatus.Active;

    public bool CanBeReserved() => IsActive;
}

using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.Infrastructure;

namespace BookingSystem.Tests.Builders;

internal sealed class RoomBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Conference Room A";
    private int _capacity = 10;
    private bool _active = true;

    public static RoomBuilder Active() => new();

    public static RoomBuilder Inactive()
    {
        var b = new RoomBuilder();
        b._active = false;
        return b;
    }

    public RoomBuilder WithId(Guid id) { _id = id; return this; }
    public RoomBuilder WithCapacity(int capacity) { _capacity = capacity; return this; }

    public Guid SeedInStore(InMemoryRoomStore store)
    {
        var room = Room.Create(
            RoomId.From(_id),
            RoomName.Create(_name),
            RoomCapacity.Create(_capacity));

        if (!_active) room.Deactivate();

        store.Execute(rooms => rooms.Add(room));
        return _id;
    }
}

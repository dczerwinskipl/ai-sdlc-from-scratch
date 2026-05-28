using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.Infrastructure;
using BookingSystem.Modules.RoomManagement.UseCases.DeactivateRoom;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.RoomManagement;

public sealed class DeactivateRoomHandlerTests
{
    private readonly InMemoryRoomStore _store = new();
    private readonly DeactivateRoomHandler _sut;

    public DeactivateRoomHandlerTests()
    {
        var repository = new InMemoryRoomRepository(_store);
        _sut = new DeactivateRoomHandler(repository);
    }

    [Fact]
    public async Task Deactivates_active_room()
    {
        // Arrange
        var roomId = RoomBuilder.Active().SeedInStore(_store);

        // Act
        var result = await _sut.Handle(new DeactivateRoomCommand(roomId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var room = _store.Execute(rooms => rooms.Single(r => r.Id.Value == roomId));
        room.Status.Should().Be(RoomStatus.Inactive);
    }

    [Fact]
    public async Task Room_remains_in_store_after_deactivation()
    {
        // Arrange
        var roomId = RoomBuilder.Active().SeedInStore(_store);

        // Act
        await _sut.Handle(new DeactivateRoomCommand(roomId), CancellationToken.None);

        // Assert
        _store.Execute(rooms => rooms.Any(r => r.Id.Value == roomId)).Should().BeTrue();
    }

    [Fact]
    public async Task Returns_not_found_when_room_does_not_exist()
    {
        // Arrange
        var command = new DeactivateRoomCommand(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }
}

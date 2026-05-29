using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.Infrastructure;
using BookingSystem.Modules.RoomManagement.UseCases.DeactivateRoom;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.Modules.RoomManagement.UseCases.DeactivateRoom;

public sealed class DeactivateRoomHandlerTests
{
    private readonly InMemoryRoomStore _store = new();
    private readonly DeactivateRoomHandler _sut;

    public DeactivateRoomHandlerTests()
    {
        _sut = new DeactivateRoomHandler(new InMemoryRoomRepository(_store));
    }

    [Fact]
    public async Task Handle_WhenRoomExists_ShouldSetStatusToInactive()
    {
        // Arrange
        var room = RoomBuilder.Active().Build();
        _store.Execute(rooms => rooms.Add(room));

        // Act
        var result = await _sut.Handle(new DeactivateRoomCommand(room.Id.Value), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        room.Status.Should().Be(RoomStatus.Inactive);
    }

    [Fact]
    public async Task Handle_WhenRoomIsDeactivated_ShouldRemainInStore()
    {
        // Arrange
        var room = RoomBuilder.Active().Build();
        _store.Execute(rooms => rooms.Add(room));

        // Act
        await _sut.Handle(new DeactivateRoomCommand(room.Id.Value), CancellationToken.None);

        // Assert
        _store.Execute(rooms => rooms.Any(r => r.Id == room.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenRoomDoesNotExist_ShouldReturnNotFoundError()
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

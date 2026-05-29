using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.Infrastructure;
using BookingSystem.Modules.RoomManagement.UseCases.AddRoom;

namespace BookingSystem.Tests.Integration.Modules.RoomManagement.UseCases.AddRoom;

public sealed class AddRoomHandlerTests
{
    private readonly InMemoryRoomStore _store = new();
    private readonly AddRoomHandler _sut;

    public AddRoomHandlerTests()
    {
        var repository = new InMemoryRoomRepository(_store);
        var validator = new AddRoomValidator();
        _sut = new AddRoomHandler(repository, validator);
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateRoomAndReturnId()
    {
        // Arrange
        var command = new AddRoomCommand("Conference Room A", 10);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.RoomId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenRoomIsCreated_ShouldBeActive()
    {
        // Arrange
        var command = new AddRoomCommand("Conference Room A", 10);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        var stored = _store.Execute(rooms => rooms.Single(r => r.Id.Value == result.Value!.RoomId));
        stored.Status.Should().Be(RoomStatus.Active);
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("  ", 10)]
    [InlineData("Room", 0)]
    [InlineData("Room", -5)]
    public async Task Handle_WhenInputIsInvalid_ShouldReturnValidationError(string name, int capacity)
    {
        // Arrange
        var command = new AddRoomCommand(name, capacity);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }
}

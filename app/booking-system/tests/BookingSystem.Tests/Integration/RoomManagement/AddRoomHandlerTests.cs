using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Infrastructure;
using BookingSystem.Modules.RoomManagement.UseCases.AddRoom;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.RoomManagement;

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
    public async Task Creates_room_and_returns_id()
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
    public async Task Created_room_is_persisted_in_store()
    {
        // Arrange
        var command = new AddRoomCommand("Conference Room A", 10);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        var stored = _store.Execute(rooms => rooms.FirstOrDefault(r => r.Id.Value == result.Value!.RoomId));
        stored.Should().NotBeNull();
        stored!.Status.Should().Be(BookingSystem.Modules.RoomManagement.Domain.RoomStatus.Active);
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("  ", 10)]
    [InlineData("Room", 0)]
    [InlineData("Room", -5)]
    public async Task Returns_validation_error_for_invalid_input(string name, int capacity)
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

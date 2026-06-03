using BookingSystem.Modules.RoomManagement.UseCases.AddRoom;

namespace BookingSystem.Tests.Unit.Modules.RoomManagement.UseCases.AddRoom;

public sealed class AddRoomValidatorTests
{
    private readonly AddRoomValidator _sut = new();

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldReturnNoError()
    {
        // Arrange
        var command = new AddRoomCommand("Conference Room A", 10);

        // Act
        var error = _sut.Validate(command);

        // Assert
        error.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenNameIsEmpty_ShouldReturnError(string name)
    {
        // Arrange
        var command = new AddRoomCommand(name, 10);

        // Act
        var error = _sut.Validate(command);

        // Assert
        error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WhenCapacityIsNotPositive_ShouldReturnError(int capacity)
    {
        // Arrange
        var command = new AddRoomCommand("Room", capacity);

        // Act
        var error = _sut.Validate(command);

        // Assert
        error.Should().NotBeNull();
    }
}

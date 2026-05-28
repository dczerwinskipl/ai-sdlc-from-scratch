using BookingSystem.Modules.RoomManagement.UseCases.AddRoom;

namespace BookingSystem.Tests.Unit.Validators;

public sealed class AddRoomValidatorTests
{
    private readonly AddRoomValidator _sut = new();

    [Fact]
    public void Returns_no_error_for_valid_command()
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
    public void Returns_error_when_name_is_empty(string name)
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
    public void Returns_error_when_capacity_is_not_positive(int capacity)
    {
        // Arrange
        var command = new AddRoomCommand("Room", capacity);

        // Act
        var error = _sut.Validate(command);

        // Assert
        error.Should().NotBeNull();
    }
}

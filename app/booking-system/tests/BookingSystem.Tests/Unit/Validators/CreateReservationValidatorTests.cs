using BookingSystem.Modules.Reservations.UseCases.CreateReservation;

namespace BookingSystem.Tests.Unit.Validators;

public sealed class CreateReservationValidatorTests
{
    private static readonly DateTimeOffset Start = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End   = new(2026, 6, 1, 11, 0, 0, TimeSpan.Zero);

    private readonly CreateReservationValidator _sut = new();

    [Fact]
    public void Returns_no_error_for_valid_command()
    {
        // Arrange
        var command = new CreateReservationCommand(Guid.NewGuid(), "Jane Doe", Start, End);

        // Act
        var error = _sut.Validate(command);

        // Assert
        error.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Returns_error_when_guest_name_is_empty(string guestName)
    {
        // Arrange
        var command = new CreateReservationCommand(Guid.NewGuid(), guestName, Start, End);

        // Act
        var error = _sut.Validate(command);

        // Assert
        error.Should().NotBeNull();
    }

    [Fact]
    public void Returns_error_when_start_is_after_end()
    {
        // Arrange
        var command = new CreateReservationCommand(Guid.NewGuid(), "Jane Doe", End, Start);

        // Act
        var error = _sut.Validate(command);

        // Assert
        error.Should().NotBeNull();
    }

    [Fact]
    public void Returns_error_when_start_equals_end()
    {
        // Arrange
        var command = new CreateReservationCommand(Guid.NewGuid(), "Jane Doe", Start, Start);

        // Act
        var error = _sut.Validate(command);

        // Assert
        error.Should().NotBeNull();
    }
}

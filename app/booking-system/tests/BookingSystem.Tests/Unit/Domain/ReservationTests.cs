using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;

namespace BookingSystem.Tests.Unit.Domain;

public sealed class ReservationTests
{
    private static Reservation CreatePending() => Reservation.Create(
        ReservationId.New(),
        ReservableRoomId.From(Guid.NewGuid()),
        ReservationGuest.Create("Jane Doe"),
        ReservationPeriod.Create(
            new DateTimeOffset(2026, 6, 1, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 1, 11, 0, 0, TimeSpan.Zero)),
        DateTimeOffset.UtcNow);

    // Confirm

    [Fact]
    public void Confirm_succeeds_for_pending_reservation()
    {
        // Arrange
        var reservation = CreatePending();

        // Act
        var result = reservation.Confirm();

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public void Confirm_returns_domain_error_when_already_confirmed()
    {
        // Arrange
        var reservation = CreatePending();
        reservation.Confirm();

        // Act
        var result = reservation.Confirm();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    [Fact]
    public void Confirm_returns_domain_error_when_cancelled()
    {
        // Arrange
        var reservation = CreatePending();
        reservation.Cancel();

        // Act
        var result = reservation.Confirm();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    // Cancel

    [Fact]
    public void Cancel_succeeds_for_pending_reservation()
    {
        // Arrange
        var reservation = CreatePending();

        // Act
        var result = reservation.Cancel();

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_succeeds_for_confirmed_reservation()
    {
        // Arrange
        var reservation = CreatePending();
        reservation.Confirm();

        // Act
        var result = reservation.Cancel();

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_returns_domain_error_when_already_cancelled()
    {
        // Arrange
        var reservation = CreatePending();
        reservation.Cancel();

        // Act
        var result = reservation.Cancel();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    // ChangePeriod

    [Fact]
    public void ChangePeriod_succeeds_for_pending_reservation()
    {
        // Arrange
        var reservation = CreatePending();
        var newPeriod = ReservationPeriod.Create(
            new DateTimeOffset(2026, 6, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 2, 12, 0, 0, TimeSpan.Zero));

        // Act
        var result = reservation.ChangePeriod(newPeriod);

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Period.Should().Be(newPeriod);
    }

    [Fact]
    public void ChangePeriod_returns_domain_error_when_cancelled()
    {
        // Arrange
        var reservation = CreatePending();
        reservation.Cancel();
        var newPeriod = ReservationPeriod.Create(
            new DateTimeOffset(2026, 6, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 2, 12, 0, 0, TimeSpan.Zero));

        // Act
        var result = reservation.ChangePeriod(newPeriod);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }
}

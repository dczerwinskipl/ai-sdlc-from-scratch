using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;

namespace BookingSystem.Tests.Unit.Modules.Reservations.Domain;

public sealed class ReservationTests
{
    private static Reservation CreatePending() => Reservation.Create(
        ReservationId.New(),
        ReservableRoomId.From(Guid.NewGuid()),
        ReservationGuest.Create("Jane Doe"),
        ReservationPeriod.Create(
            new DateTimeOffset(2026, 6, 1, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 1, 11, 0, 0, TimeSpan.Zero)).Value!,
        DateTimeOffset.UtcNow);

    // Confirm

    [Fact]
    public void Confirm_WhenReservationIsPending_ShouldSetStatusToConfirmed()
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
    public void Confirm_WhenAlreadyConfirmed_ShouldReturnDomainError()
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
    public void Confirm_WhenCancelled_ShouldReturnDomainError()
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
    public void Cancel_WhenReservationIsPending_ShouldSetStatusToCancelled()
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
    public void Cancel_WhenReservationIsConfirmed_ShouldSetStatusToCancelled()
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
    public void Cancel_WhenAlreadyCancelled_ShouldReturnDomainError()
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
    public void ChangePeriod_WhenReservationIsPending_ShouldUpdatePeriod()
    {
        // Arrange
        var reservation = CreatePending();
        var newPeriod = ReservationPeriod.Create(
            new DateTimeOffset(2026, 6, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 2, 12, 0, 0, TimeSpan.Zero)).Value!;

        // Act
        var result = reservation.ChangePeriod(newPeriod);

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Period.Should().Be(newPeriod);
    }

    [Fact]
    public void ChangePeriod_WhenConfirmed_ShouldRevertStatusToPending()
    {
        // Arrange
        var reservation = CreatePending();
        reservation.Confirm();
        var newPeriod = ReservationPeriod.Create(
            new DateTimeOffset(2026, 6, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 2, 12, 0, 0, TimeSpan.Zero)).Value!;

        // Act
        var result = reservation.ChangePeriod(newPeriod);

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Period.Should().Be(newPeriod);
        reservation.Status.Should().Be(ReservationStatus.Pending);
    }

    [Fact]
    public void ChangePeriod_WhenCancelled_ShouldReturnDomainError()
    {
        // Arrange
        var reservation = CreatePending();
        reservation.Cancel();
        var newPeriod = ReservationPeriod.Create(
            new DateTimeOffset(2026, 6, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 2, 12, 0, 0, TimeSpan.Zero)).Value!;

        // Act
        var result = reservation.ChangePeriod(newPeriod);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }
}

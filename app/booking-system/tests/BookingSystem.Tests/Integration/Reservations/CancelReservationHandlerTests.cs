using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.CancelReservation;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.Reservations;

public sealed class CancelReservationHandlerTests
{
    private readonly InMemoryReservationStore _store = new();
    private readonly CancelReservationHandler _sut;

    public CancelReservationHandlerTests()
    {
        _sut = new CancelReservationHandler(new InMemoryReservationRepository(_store));
    }

    [Fact]
    public async Task Cancels_pending_reservation()
    {
        // Arrange
        var reservationId = ReservationBuilder.Pending().SeedInStore(_store);

        // Act
        var result = await _sut.Handle(new CancelReservationCommand(reservationId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var reservation = _store.Execute(r => r.Single(x => x.Id.Value == reservationId));
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task Cancels_confirmed_reservation()
    {
        // Arrange
        var reservationId = ReservationBuilder.Confirmed().SeedInStore(_store);

        // Act
        var result = await _sut.Handle(new CancelReservationCommand(reservationId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var reservation = _store.Execute(r => r.Single(x => x.Id.Value == reservationId));
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task Returns_not_found_when_reservation_does_not_exist()
    {
        // Arrange
        var command = new CancelReservationCommand(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task Returns_domain_error_when_already_cancelled()
    {
        // Arrange
        var reservationId = ReservationBuilder.Cancelled().SeedInStore(_store);

        // Act
        var result = await _sut.Handle(new CancelReservationCommand(reservationId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }
}

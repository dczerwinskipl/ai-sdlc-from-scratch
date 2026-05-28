using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.ConfirmReservation;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.Reservations;

public sealed class ConfirmReservationHandlerTests
{
    private readonly InMemoryReservationStore _store = new();
    private readonly ConfirmReservationHandler _sut;

    public ConfirmReservationHandlerTests()
    {
        _sut = new ConfirmReservationHandler(new InMemoryReservationRepository(_store));
    }

    [Fact]
    public async Task Confirms_pending_reservation()
    {
        // Arrange
        var reservationId = ReservationBuilder.Pending().SeedInStore(_store);

        // Act
        var result = await _sut.Handle(new ConfirmReservationCommand(reservationId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var reservation = _store.Execute(r => r.Single(x => x.Id.Value == reservationId));
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public async Task Returns_not_found_when_reservation_does_not_exist()
    {
        // Arrange
        var command = new ConfirmReservationCommand(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task Returns_domain_error_when_reservation_is_already_confirmed()
    {
        // Arrange
        var reservationId = ReservationBuilder.Confirmed().SeedInStore(_store);

        // Act
        var result = await _sut.Handle(new ConfirmReservationCommand(reservationId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    [Fact]
    public async Task Returns_domain_error_when_reservation_is_cancelled()
    {
        // Arrange
        var reservationId = ReservationBuilder.Cancelled().SeedInStore(_store);

        // Act
        var result = await _sut.Handle(new ConfirmReservationCommand(reservationId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }
}

using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.CancelReservation;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.Modules.Reservations.UseCases.CancelReservation;

public sealed class CancelReservationHandlerTests
{
    private readonly InMemoryReservationStore _store = new();
    private readonly CancelReservationHandler _sut;

    public CancelReservationHandlerTests()
    {
        _sut = new CancelReservationHandler(new InMemoryReservationRepository(_store));
    }

    [Fact]
    public async Task Handle_WhenReservationIsPending_ShouldCancel()
    {
        // Arrange
        var reservation = ReservationBuilder.Pending().Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(new CancelReservationCommand(reservation.Id.Value), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_WhenReservationIsConfirmed_ShouldCancel()
    {
        // Arrange
        var reservation = ReservationBuilder.Confirmed().Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(new CancelReservationCommand(reservation.Id.Value), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public async Task Handle_WhenReservationDoesNotExist_ShouldReturnNotFoundError()
    {
        // Act
        var result = await _sut.Handle(new CancelReservationCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task Handle_WhenAlreadyCancelled_ShouldReturnDomainError()
    {
        // Arrange
        var reservation = ReservationBuilder.Cancelled().Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(new CancelReservationCommand(reservation.Id.Value), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }
}

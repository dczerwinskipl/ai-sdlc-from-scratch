using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.ConfirmReservation;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.Modules.Reservations.UseCases.ConfirmReservation;

public sealed class ConfirmReservationHandlerTests
{
    private readonly InMemoryReservationStore _store = new();
    private readonly ConfirmReservationHandler _sut;

    public ConfirmReservationHandlerTests()
    {
        _sut = new ConfirmReservationHandler(new InMemoryReservationRepository(_store));
    }

    [Fact]
    public async Task Handle_WhenReservationIsPending_ShouldConfirm()
    {
        // Arrange
        var reservation = ReservationBuilder.Pending().Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(new ConfirmReservationCommand(reservation.Id.Value), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
    }

    [Fact]
    public async Task Handle_WhenReservationDoesNotExist_ShouldReturnNotFoundError()
    {
        // Act
        var result = await _sut.Handle(new ConfirmReservationCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task Handle_WhenAlreadyConfirmed_ShouldReturnDomainError()
    {
        // Arrange
        var reservation = ReservationBuilder.Confirmed().Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(new ConfirmReservationCommand(reservation.Id.Value), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    [Fact]
    public async Task Handle_WhenCancelled_ShouldReturnDomainError()
    {
        // Arrange
        var reservation = ReservationBuilder.Cancelled().Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(new ConfirmReservationCommand(reservation.Id.Value), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }
}

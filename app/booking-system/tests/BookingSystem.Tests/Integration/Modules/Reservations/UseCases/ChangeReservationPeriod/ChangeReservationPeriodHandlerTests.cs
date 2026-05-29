using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.Modules.Reservations.UseCases.ChangeReservationPeriod;

public sealed class ChangeReservationPeriodHandlerTests
{
    private static readonly DateTimeOffset Slot1Start = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Slot1End   = new(2026, 6, 1, 11, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Slot2Start = new(2026, 6, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Slot2End   = new(2026, 6, 1, 13, 0, 0, TimeSpan.Zero);

    private readonly InMemoryReservationStore _store = new();
    private readonly ChangeReservationPeriodHandler _sut;

    public ChangeReservationPeriodHandlerTests()
    {
        var repository = new InMemoryReservationRepository(_store);
        var checker    = new InMemoryReservationAvailabilityChecker(_store);
        var validator  = new ChangeReservationPeriodValidator();
        _sut = new ChangeReservationPeriodHandler(repository, checker, validator);
    }

    [Fact]
    public async Task Handle_WhenReservationIsPending_ShouldUpdatePeriod()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var reservation = ReservationBuilder.Pending().ForRoom(roomId).WithPeriod(Slot1Start, Slot1End).Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(
            new ChangeReservationPeriodCommand(reservation.Id.Value, Slot2Start, Slot2End), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        reservation.Period.Start.Should().Be(Slot2Start);
        reservation.Period.End.Should().Be(Slot2End);
    }

    [Fact]
    public async Task Handle_WhenNewPeriodOverlapsAnotherReservation_ShouldReturnConflictError()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var reservation = ReservationBuilder.Pending().ForRoom(roomId).WithPeriod(Slot1Start, Slot1End).Build();
        var blocker     = ReservationBuilder.Pending().ForRoom(roomId).WithPeriod(Slot2Start, Slot2End).Build();
        _store.Execute(r => { r.Add(reservation); r.Add(blocker); });

        // Act
        var result = await _sut.Handle(
            new ChangeReservationPeriodCommand(reservation.Id.Value, Slot2Start, Slot2End), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ConflictError>();
    }

    [Fact]
    public async Task Handle_WhenNewPeriodOverlapsOwnSlot_ShouldSucceed()
    {
        // Arrange
        var reservation = ReservationBuilder.Pending().WithPeriod(Slot1Start, Slot1End).Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(
            new ChangeReservationPeriodCommand(
                reservation.Id.Value, Slot1Start.AddMinutes(15), Slot1End.AddMinutes(15)),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenReservationIsCancelled_ShouldReturnDomainError()
    {
        // Arrange
        var reservation = ReservationBuilder.Cancelled().WithPeriod(Slot1Start, Slot1End).Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(
            new ChangeReservationPeriodCommand(reservation.Id.Value, Slot2Start, Slot2End), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    [Fact]
    public async Task Handle_WhenReservationDoesNotExist_ShouldReturnNotFoundError()
    {
        // Act
        var result = await _sut.Handle(
            new ChangeReservationPeriodCommand(Guid.NewGuid(), Slot2Start, Slot2End), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task Handle_WhenStartIsAfterEnd_ShouldReturnValidationError()
    {
        // Arrange
        var reservation = ReservationBuilder.Pending().Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(
            new ChangeReservationPeriodCommand(reservation.Id.Value, Slot1End, Slot1Start), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }
}

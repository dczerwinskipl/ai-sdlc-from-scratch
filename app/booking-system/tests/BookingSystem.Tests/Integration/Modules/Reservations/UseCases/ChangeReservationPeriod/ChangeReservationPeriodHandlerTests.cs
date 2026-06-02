using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Infrastructure;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;
using BookingSystem.Tests.Builders;
using BookingSystem.Tests.Fakes;

namespace BookingSystem.Tests.Integration.Modules.Reservations.UseCases.ChangeReservationPeriod;

public sealed class ChangeReservationPeriodHandlerTests
{
    private static readonly DateTimeOffset Slot1Start = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Slot1End   = new(2026, 6, 1, 11, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Slot2Start = new(2026, 6, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset Slot2End   = new(2026, 6, 1, 13, 0, 0, TimeSpan.Zero);

    private readonly InMemoryReservationStore _store = new();
    private readonly InMemoryRoomStore _roomStore = new();
    private readonly ChangeReservationPeriodHandler _sut;

    public ChangeReservationPeriodHandlerTests()
    {
        var repository = new InMemoryReservationRepository(_store);
        var roomReader = new InMemoryRoomReader(_roomStore);
        var validator  = new ChangeReservationPeriodValidator(FakeClock.AtDefault());
        _sut = new ChangeReservationPeriodHandler(repository, roomReader, validator);
    }

    [Fact]
    public async Task Handle_WhenReservationIsPending_ShouldUpdatePeriod()
    {
        // Arrange
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var reservation = ReservationBuilder.Pending().ForRoom(room.Id.Value).WithPeriod(Slot1Start, Slot1End).Build();
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
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var reservation = ReservationBuilder.Pending().ForRoom(room.Id.Value).WithPeriod(Slot1Start, Slot1End).Build();
        var blocker     = ReservationBuilder.Pending().ForRoom(room.Id.Value).WithPeriod(Slot2Start, Slot2End).Build();
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
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var reservation = ReservationBuilder.Pending().ForRoom(room.Id.Value).WithPeriod(Slot1Start, Slot1End).Build();
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
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var reservation = ReservationBuilder.Cancelled().ForRoom(room.Id.Value).WithPeriod(Slot1Start, Slot1End).Build();
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

    [Fact]
    public async Task Handle_WhenRoomIsInactive_ShouldReturnConflictError()
    {
        // Arrange
        var room = RoomBuilder.Inactive().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var reservation = ReservationBuilder.Pending().ForRoom(room.Id.Value).WithPeriod(Slot1Start, Slot1End).Build();
        _store.Execute(r => r.Add(reservation));

        // Act
        var result = await _sut.Handle(
            new ChangeReservationPeriodCommand(reservation.Id.Value, Slot2Start, Slot2End), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ConflictError>();
    }
}

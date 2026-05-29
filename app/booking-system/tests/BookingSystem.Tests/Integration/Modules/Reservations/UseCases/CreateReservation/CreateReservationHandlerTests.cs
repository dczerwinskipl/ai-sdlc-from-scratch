using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Infrastructure;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.CreateReservation;
using BookingSystem.Tests.Builders;
using BookingSystem.Tests.Fakes;

namespace BookingSystem.Tests.Integration.Modules.Reservations.UseCases.CreateReservation;

public sealed class CreateReservationHandlerTests
{
    private static readonly DateTimeOffset Start = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset End   = new(2026, 6, 1, 11, 0, 0, TimeSpan.Zero);

    private readonly InMemoryRoomStore _roomStore = new();
    private readonly InMemoryReservationStore _reservationStore = new();
    private readonly CreateReservationHandler _sut;

    public CreateReservationHandlerTests()
    {
        var reservationRepository = new InMemoryReservationRepository(_reservationStore);
        var availabilityChecker   = new InMemoryReservationAvailabilityChecker(_reservationStore);
        var roomReader             = new InMemoryRoomReader(_roomStore);
        var validator              = new CreateReservationValidator();
        _sut = new CreateReservationHandler(
            reservationRepository, availabilityChecker, roomReader, validator, FakeClock.AtDefault());
    }

    [Fact]
    public async Task Handle_WhenRoomIsActiveAndPeriodIsAvailable_ShouldCreateReservation()
    {
        // Arrange
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var command = new CreateReservationCommand(room.Id.Value, "Jane Doe", Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ReservationId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenReservationIsCreated_ShouldHavePendingStatus()
    {
        // Arrange
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var command = new CreateReservationCommand(room.Id.Value, "Jane Doe", Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        var reservation = _reservationStore.Execute(r => r.Single(x => x.Id.Value == result.Value!.ReservationId));
        reservation.Status.Should().Be(ReservationStatus.Pending);
    }

    [Fact]
    public async Task Handle_WhenRoomDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new CreateReservationCommand(Guid.NewGuid(), "Jane Doe", Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task Handle_WhenRoomIsInactive_ShouldReturnConflictError()
    {
        // Arrange
        var room = RoomBuilder.Inactive().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var command = new CreateReservationCommand(room.Id.Value, "Jane Doe", Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ConflictError>();
    }

    [Fact]
    public async Task Handle_WhenPeriodOverlapsExistingReservation_ShouldReturnConflictError()
    {
        // Arrange
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));

        var existing = ReservationBuilder.Pending().ForRoom(room.Id.Value).WithPeriod(Start, End).Build();
        _reservationStore.Execute(r => r.Add(existing));

        var command = new CreateReservationCommand(room.Id.Value, "John Doe", Start.AddMinutes(30), End.AddMinutes(30));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ConflictError>();
    }

    [Fact]
    public async Task Handle_WhenPeriodIsAdjacentToExistingReservation_ShouldSucceed()
    {
        // Arrange
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));

        var existing = ReservationBuilder.Pending().ForRoom(room.Id.Value).WithPeriod(Start, End).Build();
        _reservationStore.Execute(r => r.Add(existing));

        var command = new CreateReservationCommand(room.Id.Value, "John Doe", End, End.AddHours(1));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WhenGuestNameIsEmpty_ShouldReturnValidationError(string guestName)
    {
        // Arrange
        var room = RoomBuilder.Active().Build();
        _roomStore.Execute(rooms => rooms.Add(room));
        var command = new CreateReservationCommand(room.Id.Value, guestName, Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }
}

using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Infrastructure;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.CreateReservation;
using BookingSystem.Tests.Builders;
using BookingSystem.Tests.Fakes;

namespace BookingSystem.Tests.Integration.Reservations;

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
    public async Task Creates_reservation_when_room_is_active_and_period_is_available()
    {
        // Arrange
        var roomId = RoomBuilder.Active().SeedInStore(_roomStore);
        var command = new CreateReservationCommand(roomId, "Jane Doe", Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.ReservationId.Should().NotBeEmpty();
    }

    [Fact]
    public async Task New_reservation_is_pending()
    {
        // Arrange
        var roomId = RoomBuilder.Active().SeedInStore(_roomStore);
        var command = new CreateReservationCommand(roomId, "Jane Doe", Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        var reservation = _reservationStore.Execute(r => r.Single(x => x.Id.Value == result.Value!.ReservationId));
        reservation.Status.Should().Be(ReservationStatus.Pending);
    }

    [Fact]
    public async Task Returns_not_found_when_room_does_not_exist()
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
    public async Task Returns_conflict_when_room_is_inactive()
    {
        // Arrange
        var roomId = RoomBuilder.Inactive().SeedInStore(_roomStore);
        var command = new CreateReservationCommand(roomId, "Jane Doe", Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ConflictError>();
    }

    [Fact]
    public async Task Returns_conflict_when_period_overlaps_existing_reservation()
    {
        // Arrange
        var roomId = RoomBuilder.Active().SeedInStore(_roomStore);
        ReservationBuilder.Pending()
            .ForRoom(roomId)
            .WithPeriod(Start, End)
            .SeedInStore(_reservationStore);

        var overlapping = new CreateReservationCommand(roomId, "John Doe", Start.AddMinutes(30), End.AddMinutes(30));

        // Act
        var result = await _sut.Handle(overlapping, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ConflictError>();
    }

    [Fact]
    public async Task Allows_reservation_for_adjacent_period_on_same_room()
    {
        // Arrange
        var roomId = RoomBuilder.Active().SeedInStore(_roomStore);
        ReservationBuilder.Pending()
            .ForRoom(roomId)
            .WithPeriod(Start, End)
            .SeedInStore(_reservationStore);

        var adjacent = new CreateReservationCommand(roomId, "John Doe", End, End.AddHours(1));

        // Act
        var result = await _sut.Handle(adjacent, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Returns_validation_error_when_guest_name_is_empty(string guestName)
    {
        // Arrange
        var roomId = RoomBuilder.Active().SeedInStore(_roomStore);
        var command = new CreateReservationCommand(roomId, guestName, Start, End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }
}

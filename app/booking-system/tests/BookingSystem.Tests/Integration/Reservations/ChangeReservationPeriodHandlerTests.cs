using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;
using BookingSystem.Tests.Builders;

namespace BookingSystem.Tests.Integration.Reservations;

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
    public async Task Updates_period_for_pending_reservation()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var reservationId = ReservationBuilder.Pending()
            .ForRoom(roomId)
            .WithPeriod(Slot1Start, Slot1End)
            .SeedInStore(_store);

        var command = new ChangeReservationPeriodCommand(reservationId, Slot2Start, Slot2End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var reservation = _store.Execute(r => r.Single(x => x.Id.Value == reservationId));
        reservation.Period.Start.Should().Be(Slot2Start);
        reservation.Period.End.Should().Be(Slot2End);
    }

    [Fact]
    public async Task Returns_conflict_when_new_period_overlaps_another_reservation()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var reservationId = ReservationBuilder.Pending()
            .ForRoom(roomId)
            .WithPeriod(Slot1Start, Slot1End)
            .SeedInStore(_store);

        ReservationBuilder.Pending()
            .ForRoom(roomId)
            .WithPeriod(Slot2Start, Slot2End)
            .SeedInStore(_store);

        var command = new ChangeReservationPeriodCommand(reservationId, Slot2Start, Slot2End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ConflictError>();
    }

    [Fact]
    public async Task Allows_rescheduling_within_same_slot_for_own_reservation()
    {
        // Arrange
        var reservationId = ReservationBuilder.Pending()
            .WithPeriod(Slot1Start, Slot1End)
            .SeedInStore(_store);

        var command = new ChangeReservationPeriodCommand(
            reservationId, Slot1Start.AddMinutes(15), Slot1End.AddMinutes(15));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Returns_domain_error_when_reservation_is_cancelled()
    {
        // Arrange
        var reservationId = ReservationBuilder.Cancelled()
            .WithPeriod(Slot1Start, Slot1End)
            .SeedInStore(_store);

        var command = new ChangeReservationPeriodCommand(reservationId, Slot2Start, Slot2End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    [Fact]
    public async Task Returns_not_found_when_reservation_does_not_exist()
    {
        // Arrange
        var command = new ChangeReservationPeriodCommand(Guid.NewGuid(), Slot2Start, Slot2End);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFoundError>();
    }

    [Fact]
    public async Task Returns_validation_error_when_start_is_after_end()
    {
        // Arrange
        var reservationId = ReservationBuilder.Pending().SeedInStore(_store);
        var command = new ChangeReservationPeriodCommand(reservationId, Slot1End, Slot1Start);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
    }
}

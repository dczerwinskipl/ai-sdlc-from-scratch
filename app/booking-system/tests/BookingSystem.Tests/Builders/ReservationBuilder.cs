using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.Infrastructure;

namespace BookingSystem.Tests.Builders;

internal sealed class ReservationBuilder
{
    private static readonly DateTimeOffset DefaultStart = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset DefaultEnd   = new(2026, 6, 1, 11, 0, 0, TimeSpan.Zero);

    private Guid _id = Guid.NewGuid();
    private Guid _roomId = Guid.NewGuid();
    private string _guestName = "Jane Doe";
    private DateTimeOffset _start = DefaultStart;
    private DateTimeOffset _end = DefaultEnd;
    private ReservationStatus _targetStatus = ReservationStatus.Pending;

    // Named scenarios

    public static ReservationBuilder Pending() => new();

    public static ReservationBuilder Confirmed()
    {
        var b = new ReservationBuilder();
        b._targetStatus = ReservationStatus.Confirmed;
        return b;
    }

    public static ReservationBuilder Cancelled()
    {
        var b = new ReservationBuilder();
        b._targetStatus = ReservationStatus.Cancelled;
        return b;
    }

    // Fluent customisation

    public ReservationBuilder ForRoom(Guid roomId) { _roomId = roomId; return this; }
    public ReservationBuilder WithId(Guid id) { _id = id; return this; }
    public ReservationBuilder WithPeriod(DateTimeOffset start, DateTimeOffset end) { _start = start; _end = end; return this; }

    // Store seeding

    public Guid SeedInStore(InMemoryReservationStore store)
    {
        var reservation = Reservation.Create(
            ReservationId.From(_id),
            ReservableRoomId.From(_roomId),
            ReservationGuest.Create(_guestName),
            ReservationPeriod.Create(_start, _end),
            DateTimeOffset.UtcNow);

        if (_targetStatus == ReservationStatus.Confirmed) reservation.Confirm();
        if (_targetStatus == ReservationStatus.Cancelled) reservation.Cancel();

        store.Execute(r => r.Add(reservation));
        return _id;
    }
}

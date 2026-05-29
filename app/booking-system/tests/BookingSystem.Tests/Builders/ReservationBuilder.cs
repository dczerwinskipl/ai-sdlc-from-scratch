using BookingSystem.Modules.Reservations.Domain;

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

    public static ReservationBuilder Pending() => new();
    public static ReservationBuilder Confirmed() => new() { _targetStatus = ReservationStatus.Confirmed };
    public static ReservationBuilder Cancelled() => new() { _targetStatus = ReservationStatus.Cancelled };

    public ReservationBuilder ForRoom(Guid roomId) { _roomId = roomId; return this; }
    public ReservationBuilder WithId(Guid id) { _id = id; return this; }
    public ReservationBuilder WithPeriod(DateTimeOffset start, DateTimeOffset end) { _start = start; _end = end; return this; }

    public Reservation Build()
    {
        var reservation = Reservation.Create(
            ReservationId.From(_id),
            ReservableRoomId.From(_roomId),
            ReservationGuest.Create(_guestName),
            ReservationPeriod.Create(_start, _end),
            DateTimeOffset.UtcNow);

        if (_targetStatus == ReservationStatus.Confirmed) reservation.Confirm();
        if (_targetStatus == ReservationStatus.Cancelled) reservation.Cancel();

        return reservation;
    }
}

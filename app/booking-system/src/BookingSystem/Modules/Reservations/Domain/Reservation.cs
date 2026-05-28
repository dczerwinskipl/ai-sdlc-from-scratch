using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.Reservations.Domain;

internal sealed class Reservation : AggregateRoot<ReservationId>
{
    public ReservableRoomId RoomId { get; private set; } = null!;
    public ReservationGuest Guest { get; private set; } = null!;
    public ReservationPeriod Period { get; private set; } = null!;
    public ReservationStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Reservation() { }

    public static Reservation Create(
        ReservationId id,
        ReservableRoomId roomId,
        ReservationGuest guest,
        ReservationPeriod period,
        DateTimeOffset createdAt)
    {
        var reservation = new Reservation
        {
            Id = id,
            Status = ReservationStatus.Pending,
            CreatedAt = createdAt
        };
        reservation.RoomId = roomId;
        reservation.Guest = guest;
        reservation.Period = period;
        return reservation;
    }

    public Result Confirm()
    {
        if (Status != ReservationStatus.Pending)
            return new DomainError("Only pending reservations can be confirmed.");
        Status = ReservationStatus.Confirmed;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == ReservationStatus.Cancelled)
            return new DomainError("Reservation is already cancelled.");
        Status = ReservationStatus.Cancelled;
        return Result.Success();
    }

    public Result ChangePeriod(ReservationPeriod newPeriod)
    {
        if (Status == ReservationStatus.Cancelled)
            return new DomainError("Cannot change period of a cancelled reservation.");
        Period = newPeriod;
        return Result.Success();
    }

    public bool IsActive() => Status != ReservationStatus.Cancelled;
}

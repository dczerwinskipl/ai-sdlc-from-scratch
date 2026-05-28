using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.Reservations.Domain;

internal sealed class ReservationPeriod : ValueObject
{
    public DateTimeOffset Start { get; }
    public DateTimeOffset End { get; }

    private ReservationPeriod(DateTimeOffset start, DateTimeOffset end)
    {
        Start = start;
        End = end;
    }

    public static ReservationPeriod Create(DateTimeOffset start, DateTimeOffset end)
    {
        if (start >= end)
            throw new DomainException("Start must be before end.");
        return new ReservationPeriod(start, end);
    }

    public bool Overlaps(ReservationPeriod other) =>
        Start < other.End && other.Start < End;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Start;
        yield return End;
    }
}

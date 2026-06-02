using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.Reservations.Domain;

internal sealed class ReservationGuest : ValueObject
{
    public string Name { get; }

    private ReservationGuest(string name) => Name = name;

    public static ReservationGuest Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Guest name cannot be empty.");
        return new ReservationGuest(name.Trim());
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
    }
}

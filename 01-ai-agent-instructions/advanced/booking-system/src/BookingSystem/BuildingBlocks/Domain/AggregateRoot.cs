namespace BookingSystem.BuildingBlocks.Domain;

internal abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<DomainEvent> _domainEvents = [];

    protected AggregateRoot() { }
    protected AggregateRoot(TId id) : base(id) { }

    protected void RaiseDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
}

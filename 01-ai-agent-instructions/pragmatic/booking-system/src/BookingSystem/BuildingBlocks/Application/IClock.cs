namespace BookingSystem.BuildingBlocks.Application;

internal interface IClock
{
    DateTimeOffset UtcNow { get; }
}

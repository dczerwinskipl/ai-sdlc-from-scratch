using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Tests.Fakes;

internal sealed class FakeClock(DateTimeOffset utcNow) : IClock
{
    public static readonly DateTimeOffset Default = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public static FakeClock At(DateTimeOffset utcNow) => new(utcNow);
    public static FakeClock AtDefault() => new(Default);

    public DateTimeOffset UtcNow => utcNow;
}

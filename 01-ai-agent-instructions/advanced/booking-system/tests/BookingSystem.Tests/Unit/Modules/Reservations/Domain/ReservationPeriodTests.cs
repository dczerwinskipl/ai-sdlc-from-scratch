using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;

namespace BookingSystem.Tests.Unit.Modules.Reservations.Domain;

public sealed class ReservationPeriodTests
{
    private static readonly DateTimeOffset Base = new(2026, 6, 1, 10, 0, 0, TimeSpan.Zero);

    // Creation guards

    [Fact]
    public void Create_WhenStartIsBeforeEnd_ShouldSucceed()
    {
        // Arrange / Act
        var result = ReservationPeriod.Create(Base, Base.AddHours(1));

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Start.Should().Be(Base);
        result.Value!.End.Should().Be(Base.AddHours(1));
    }

    [Fact]
    public void Create_WhenStartEqualsEnd_ShouldReturnFailure()
    {
        // Arrange / Act
        var result = ReservationPeriod.Create(Base, Base);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    [Fact]
    public void Create_WhenStartIsAfterEnd_ShouldReturnFailure()
    {
        // Arrange / Act
        var result = ReservationPeriod.Create(Base.AddHours(1), Base);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<DomainError>();
    }

    // Overlap logic

    [Fact]
    public void Overlaps_WhenPeriodsFullyOverlap_ShouldReturnTrue()
    {
        // Arrange
        var a = ReservationPeriod.Create(Base, Base.AddHours(2)).Value!;
        var b = ReservationPeriod.Create(Base.AddMinutes(30), Base.AddMinutes(90)).Value!;

        // Act / Assert
        a.Overlaps(b).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_WhenPeriodsPartiallyOverlap_ShouldReturnTrue()
    {
        // Arrange
        var a = ReservationPeriod.Create(Base, Base.AddHours(2)).Value!;
        var b = ReservationPeriod.Create(Base.AddHours(1), Base.AddHours(3)).Value!;

        // Act / Assert
        a.Overlaps(b).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_WhenPeriodsAreAdjacent_ShouldReturnFalse()
    {
        // Arrange
        var a = ReservationPeriod.Create(Base, Base.AddHours(1)).Value!;
        var b = ReservationPeriod.Create(Base.AddHours(1), Base.AddHours(2)).Value!;

        // Act / Assert
        a.Overlaps(b).Should().BeFalse();
    }

    [Fact]
    public void Overlaps_WhenPeriodsDoNotOverlap_ShouldReturnFalse()
    {
        // Arrange
        var a = ReservationPeriod.Create(Base, Base.AddHours(1)).Value!;
        var b = ReservationPeriod.Create(Base.AddHours(2), Base.AddHours(3)).Value!;

        // Act / Assert
        a.Overlaps(b).Should().BeFalse();
    }
}

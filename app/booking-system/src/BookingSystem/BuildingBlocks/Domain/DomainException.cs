namespace BookingSystem.BuildingBlocks.Domain;

internal sealed class DomainException(string message) : Exception(message);

namespace BookingSystem.BuildingBlocks.Domain;

internal abstract record Error(string Message);

internal sealed record ValidationError(string Message) : Error(Message);
internal sealed record NotFoundError(string Message) : Error(Message);
internal sealed record ConflictError(string Message) : Error(Message);

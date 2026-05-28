using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.Reservations.UseCases.CreateReservation;

internal static class CreateReservationEndpoint
{
    public static IEndpointRouteBuilder MapCreateReservationEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapPost("/", async (
            CreateReservationRequest request,
            CreateReservationHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateReservationCommand(request.RoomId, request.GuestName, request.Start, request.End);
            var result = await handler.Handle(command, cancellationToken);
            return result.ToHttpResult(r => Results.Created($"/api/reservations/{r.ReservationId}", r));
        });

        return group;
    }
}

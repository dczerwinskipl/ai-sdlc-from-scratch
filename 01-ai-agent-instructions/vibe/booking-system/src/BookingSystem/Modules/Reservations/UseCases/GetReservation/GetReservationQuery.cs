using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.Reservations.UseCases.GetReservation;

internal sealed record GetReservationQuery(Guid ReservationId) : IQuery<GetReservationResponse?>;

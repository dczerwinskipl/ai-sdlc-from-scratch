using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.Reservations.UseCases.ListReservations;

internal sealed record ListReservationsQuery : IQuery<IReadOnlyList<ReservationListItem>>;

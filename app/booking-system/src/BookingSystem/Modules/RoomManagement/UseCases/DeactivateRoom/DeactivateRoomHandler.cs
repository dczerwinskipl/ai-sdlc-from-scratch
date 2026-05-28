using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

namespace BookingSystem.Modules.RoomManagement.UseCases.DeactivateRoom;

internal sealed class DeactivateRoomHandler(
    IRoomRepository repository) : ICommandHandler<DeactivateRoomCommand>
{
    public async Task Handle(DeactivateRoomCommand command, CancellationToken cancellationToken)
    {
        var room = await repository.GetById(RoomId.From(command.RoomId), cancellationToken)
            ?? throw new KeyNotFoundException($"Room {command.RoomId} not found.");

        room.Deactivate();

        await repository.Update(room, cancellationToken);
    }
}

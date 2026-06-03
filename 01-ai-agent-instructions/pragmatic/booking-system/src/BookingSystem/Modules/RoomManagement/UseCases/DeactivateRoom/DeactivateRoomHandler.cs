using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

namespace BookingSystem.Modules.RoomManagement.UseCases.DeactivateRoom;

internal sealed class DeactivateRoomHandler(
    IRoomRepository repository) : ICommandHandler<DeactivateRoomCommand>
{
    public async Task<Result> Handle(DeactivateRoomCommand command, CancellationToken cancellationToken)
    {
        var room = await repository.GetById(RoomId.From(command.RoomId), cancellationToken);
        if (room is null)
            return new NotFoundError($"Room {command.RoomId} not found.");

        room.Deactivate();
        await repository.Update(room, cancellationToken);
        return Result.Success();
    }
}

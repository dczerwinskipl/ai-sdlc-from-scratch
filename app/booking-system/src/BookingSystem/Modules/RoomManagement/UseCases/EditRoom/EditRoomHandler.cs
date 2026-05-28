using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

namespace BookingSystem.Modules.RoomManagement.UseCases.EditRoom;

internal sealed class EditRoomHandler(
    IRoomRepository repository,
    EditRoomValidator validator) : ICommandHandler<EditRoomCommand, EditRoomResponse>
{
    public async Task<Result<EditRoomResponse>> Handle(EditRoomCommand command, CancellationToken cancellationToken)
    {
        var error = validator.Validate(command);
        if (error is not null)
            return new ValidationError(error);

        var room = await repository.GetById(RoomId.From(command.RoomId), cancellationToken);
        if (room is null)
            return new NotFoundError($"Room {command.RoomId} not found.");

        room.Rename(RoomName.Create(command.Name));
        room.ChangeCapacity(RoomCapacity.Create(command.Capacity));
        await repository.Update(room, cancellationToken);
        return new EditRoomResponse(room.Id.Value);
    }
}

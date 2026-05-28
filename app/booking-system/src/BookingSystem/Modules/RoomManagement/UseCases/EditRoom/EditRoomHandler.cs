using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

namespace BookingSystem.Modules.RoomManagement.UseCases.EditRoom;

internal sealed class EditRoomHandler(
    IRoomRepository repository,
    EditRoomValidator validator) : ICommandHandler<EditRoomCommand, EditRoomResponse>
{
    public async Task<EditRoomResponse> Handle(EditRoomCommand command, CancellationToken cancellationToken)
    {
        var error = validator.Validate(command);
        if (error is not null)
            throw new ArgumentException(error);

        var room = await repository.GetById(RoomId.From(command.RoomId), cancellationToken)
            ?? throw new KeyNotFoundException($"Room {command.RoomId} not found.");

        room.Rename(RoomName.Create(command.Name));
        room.ChangeCapacity(RoomCapacity.Create(command.Capacity));

        await repository.Update(room, cancellationToken);

        return new EditRoomResponse(room.Id.Value);
    }
}

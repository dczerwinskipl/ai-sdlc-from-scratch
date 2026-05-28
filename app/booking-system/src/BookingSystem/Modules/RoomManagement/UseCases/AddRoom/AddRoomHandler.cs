using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;

namespace BookingSystem.Modules.RoomManagement.UseCases.AddRoom;

internal sealed class AddRoomHandler(
    IRoomRepository repository,
    AddRoomValidator validator) : ICommandHandler<AddRoomCommand, AddRoomResponse>
{
    public async Task<AddRoomResponse> Handle(AddRoomCommand command, CancellationToken cancellationToken)
    {
        var error = validator.Validate(command);
        if (error is not null)
            throw new ArgumentException(error);

        var room = Room.Create(
            RoomId.New(),
            RoomName.Create(command.Name),
            RoomCapacity.Create(command.Capacity));

        await repository.Add(room, cancellationToken);

        return new AddRoomResponse(room.Id.Value);
    }
}

using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.BuildingBlocks.Api;

internal static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess) =>
        result.IsSuccess
            ? onSuccess(result.Value!)
            : result.Error!.ToHttpResult();

    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess
            ? Results.NoContent()
            : result.Error!.ToHttpResult();

    public static IResult ToHttpResult(this Error error) => error switch
    {
        ValidationError e => Results.Problem(e.Message, statusCode: StatusCodes.Status400BadRequest),
        NotFoundError e   => Results.Problem(e.Message, statusCode: StatusCodes.Status404NotFound),
        ConflictError e   => Results.Problem(e.Message, statusCode: StatusCodes.Status409Conflict),
        _                 => Results.Problem(statusCode: StatusCodes.Status500InternalServerError)
    };
}

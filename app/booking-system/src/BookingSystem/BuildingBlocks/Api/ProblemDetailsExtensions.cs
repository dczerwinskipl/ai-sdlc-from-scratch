namespace BookingSystem.BuildingBlocks.Api;

internal static class ProblemDetailsExtensions
{
    public static IResult ValidationProblem(string detail) =>
        Results.Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest);

    public static IResult NotFoundProblem(string detail) =>
        Results.Problem(detail: detail, statusCode: StatusCodes.Status404NotFound);

    public static IResult ConflictProblem(string detail) =>
        Results.Problem(detail: detail, statusCode: StatusCodes.Status409Conflict);
}

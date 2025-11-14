namespace DiffErgoSum.Api.Exceptions;

public sealed class DiffNotFoundHttpException(int id) : HttpException(
    statusCode: StatusCodes.Status404NotFound,
    title: "DiffNotFound",
    detail: $"Diff with ID {id} was not found."
);

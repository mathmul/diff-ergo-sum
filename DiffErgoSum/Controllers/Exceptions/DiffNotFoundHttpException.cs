namespace DiffErgoSum.Controllers.Exceptions;

public sealed class DiffNotFoundHttpException(int id) : HttpException(
    StatusCodes.Status404NotFound,
    "DiffNotFound",
    $"Diff with ID {id} was not found."
);

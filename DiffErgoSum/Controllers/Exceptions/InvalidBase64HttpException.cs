namespace DiffErgoSum.Controllers.Exceptions;

public sealed class InvalidBase64HttpException() : HttpException(
    StatusCodes.Status422UnprocessableEntity,
    "InvalidBase64",
    "Provided data is not valid Base64."
);

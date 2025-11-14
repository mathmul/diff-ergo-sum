namespace DiffErgoSum.Api.Exceptions;

public sealed class InvalidBase64HttpException() : HttpException(
    statusCode: StatusCodes.Status422UnprocessableEntity,
    title: "Invalid Base64 Input",
    detail: "Provided data is not valid Base64."
);

namespace DiffErgoSum.Api.Exceptions;

using DiffErgoSum.Core.Constants;

public sealed class InvalidBase64HttpException() : HttpException(
    statusCode: StatusCodes.Status422UnprocessableEntity,
    title: "Invalid Base64 Input",
    detail: ValidationMessages.InvalidBase64
);

namespace Mycelium.Api.Core.Results;

public enum ErrorType
{
    None,
    Validation,
    NotFound,
    Unauthorized,
    Forbidden,
    Conflict,
    Unexpected
}

public sealed record Error(ErrorType Type, string Message)
{
    public static readonly Error None = new(ErrorType.None, string.Empty);

    public static Error Validation(string message) => new(ErrorType.Validation, message);
    public static Error NotFound(string message) => new(ErrorType.NotFound, message);
    public static Error Unauthorized(string message) => new(ErrorType.Unauthorized, message);
    public static Error Forbidden(string message) => new(ErrorType.Forbidden, message);
    public static Error Conflict(string message) => new(ErrorType.Conflict, message);
    public static Error Unexpected(string message) => new(ErrorType.Unexpected, message);
}

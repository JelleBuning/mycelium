namespace Mycelium.Api.Core.Results;

public class Result
{
    public Error Error { get; }
    public bool IsSuccess => Error == Error.None;
    public bool IsFailure => !IsSuccess;

    protected Result(Error error)
    {
        Error = error;
    }

    public static Result Success() => new(Error.None);
    public static Result Failure(Error error) => new(error);

    public static Result<T> Success<T>(T value) => new(value, Error.None);
    public static Result<T> Failure<T>(Error error) => new(default, error);
}

public sealed class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T? value, Error error) : base(error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    public static implicit operator Result<T>(T value) => Success(value);
}

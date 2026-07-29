using System.Diagnostics.CodeAnalysis;

namespace LingoHop.Application.Common;

/// <summary>
/// Outcome of a use case. Expected failures (missing deck, duplicate word) travel as data
/// rather than exceptions, so controllers can map them to ProblemDetails without try/catch.
/// </summary>
public class Result
{
    protected Result(Error? error) => Error = error;

    public Error? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);

    public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.FromValue(value);

    public static Result<TValue> Failure<TValue>(Error error) => Result<TValue>.FromError(error);
}

/// <inheritdoc cref="Result"/>
/// <typeparam name="TValue">Payload produced on success.</typeparam>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    private Result(TValue? value, Error? error) : base(error) => _value = value;

    /// <summary>The payload. Only valid when <see cref="Result.IsSuccess"/> is <c>true</c>.</summary>
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failed result cannot be read.");

    internal static Result<TValue> FromValue(TValue value) => new(value, null);

    internal static Result<TValue> FromError(Error error) => new(default, error);

    public static implicit operator Result<TValue>(TValue value) => FromValue(value);

    public static implicit operator Result<TValue>(Error error) => FromError(error);
}

namespace SwedishTestSsn.Client;

/// <summary>
/// Describes why an operation failed.
/// </summary>
/// <param name="Message">Human readable error message.</param>
public sealed record Error(string Message);

/// <summary>
/// Result of an operation that either succeeded with a value or failed with an <see cref="Client.Error"/>.
/// </summary>
/// <typeparam name="T">Type of the success value.</typeparam>
public readonly record struct Result<T>
{
    private readonly T? _value;
    private readonly Error? _error;

    private Result(T value)
    {
        _value = value;
        IsSuccess = true;
    }

    private Result(Error error)
    {
        _error = error;
    }

    /// <summary>
    /// <see langword="true"/> if the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The success value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is a failure.</exception>
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Result has no value.");

    /// <summary>
    /// The error.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the result is a success.</exception>
    public Error Error => IsSuccess ? throw new InvalidOperationException("Result has no error.") : _error!;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The success value.</param>
    public static Result<T> Success(T value) => new(value);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error.</param>
    public static Result<T> Failure(Error error) => new(error);

    /// <summary>
    /// Invokes <paramref name="onSuccess"/> or <paramref name="onFailure"/> depending on the outcome.
    /// </summary>
    /// <typeparam name="TOut">Type of the returned value.</typeparam>
    /// <param name="onSuccess">Called with the value on success.</param>
    /// <param name="onFailure">Called with the error on failure.</param>
    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(_error!);
}

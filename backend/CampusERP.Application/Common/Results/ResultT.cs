namespace CampusERP.Application.Common.Results;

public class Result<T> : Result
{
    private Result(T? value, bool isSuccess, Error? error, IEnumerable<Error>? errors = null) : base(isSuccess, error, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(value, true, null);

    public new static Result<T> Failure(Error error) => new(default, false, error);

    public new static Result<T> Failure(IEnumerable<Error> errors) => new(default, false, errors.FirstOrDefault(), errors);
}
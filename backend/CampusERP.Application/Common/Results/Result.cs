namespace CampusERP.Application.Common.Results;

public class Result
{
    protected Result(bool isSuccess, Error? error, IEnumerable<Error>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors?.ToList() ?? [];
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error? Error { get; }

    public IReadOnlyCollection<Error> Errors { get; }

    public static Result Success() => new(true, null);

    public static Result Failure(Error error) => new(false, error);

    public static Result Failure(IEnumerable<Error> errors) => new(false, errors.FirstOrDefault(), errors);
}
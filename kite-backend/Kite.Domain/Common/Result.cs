namespace Kite.Domain.Common;

public class Result<T>
{
    private readonly List<Error> _errors = new();
    public bool IsSuccess { get; }
    public T? Value { get; }

    public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

    private Result(bool isSuccess, T? value, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Value = value;

        if (errors.Any())
        {
            _errors.AddRange(errors);
        }
    }

    public static Result<T> Success(T? value = default)
    {
        return new Result<T>(isSuccess: true, value: value, errors: new List<Error>());
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(isSuccess: false, value: default,
            errors: new List<Error> { new Error("General.Error", error) });
    }

    public static Result<T> Failure(params Error[] errors)
    {
        return new Result<T>(isSuccess: false, value: default, errors: errors);
    }
}
namespace WordsCloudGenerator.models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string Error { get; }
    
    private Result(T value, bool isSuccess, string error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result<T> Ok(T value) => new Result<T>(value, true, null);
    public static Result<T> Fail(string error) => new Result<T>(default, false, error);
    
    public Result<TResult> Then<TResult>(Func<T, Result<TResult>> func)
        => IsSuccess ? func(Value) : Result<TResult>.Fail(Error);
    
    public T OnFailure(T defaultValue)
        => IsSuccess ? Value : defaultValue;
}

public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    
    private Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result Ok() => new Result(true, null);
    public static Result Fail(string error) => new Result(false, error);
}
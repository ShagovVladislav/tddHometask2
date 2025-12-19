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
    
    public Result<TResult> Map<TResult>(Func<T, TResult> func)
        => IsSuccess ? Result<TResult>.Ok(func(Value)) : Result<TResult>.Fail(Error);
    
    public Result Then(Func<T, Result> func)
        => IsSuccess ? func(Value) : Result.Fail(Error);

    public T OnFailure(T defaultValue)
        => IsSuccess ? Value : defaultValue;
    
    public T OnFailure(Func<T> defaultValueFactory)
        => IsSuccess ? Value : defaultValueFactory();
    
    public void Match(Action<T> onSuccess, Action<string> onFailure)
    {
        if (IsSuccess)
            onSuccess(Value);
        else
            onFailure(Error);
    }
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
    
    public Result<T> Then<T>(Func<Result<T>> func)
        => IsSuccess ? func() : Result<T>.Fail(Error);
    
    public Result Then(Func<Result> func)
        => IsSuccess ? func() : Fail(Error);
}
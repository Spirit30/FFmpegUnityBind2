using System;

public class Result<T>
{
    public bool IsSuccess { get; protected set; }
    public T Data { get; protected set; }
    public Exception Exception { get; protected set; }

    public static Result<T> Success(T data)
    {
        return new Result<T>()
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static Result<T> Failure(Exception ex)
    {
        return new Result<T>()
        {
            IsSuccess = false,
            Exception = ex
        };
    }
}

public class Result : Result<object>
{
    public static Result Success()
    {
        return new Result()
        {
            IsSuccess = true
        };
    }

    public static new Result Failure(Exception ex)
    {
        return new Result()
        {
            IsSuccess = false,
            Exception = ex
        };
    }

    public static Result Failure(string error)
    {
        return Failure(new Exception(error));
    }
}
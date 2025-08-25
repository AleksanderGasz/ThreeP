namespace Mac.Modules.Results;

public record ResultCustomService(bool Success, string? Message)
{
    public static ResultCustomService OK(string message = "") => new(true, message);
    public static ResultCustomService<T> OK<T>(T data, string? message = "") => new(data, true, message);

    public static ResultCustomService Error(string message, bool isLog = true)
    {
        if (isLog) LogError<ResultCustomService>(message);
        return new ResultCustomService(false, message);
    }

    public static ResultCustomService<T> Error<T>(string message, bool isLog = true)
    {
        if (isLog) LogError<T>(message);
        return new ResultCustomService<T>(default, false, message);
    }
    // static void LogError<T>(string e) => Log.Logger.ForContext<T>().Error(e);
    static void LogError<T>(string e) => StaticLogger.Log.LogError(e);
}

public record ResultCustomService<T>(T? Data, bool Success, string? Message) : ResultCustomService(Success, Message);

/*public class Result
{
    protected bool success { get; set; }
    protected string? message { get; set; }

    public static Result OK(string message) => new() { success = true, message = message };
    public static Result Error(string message) => new() { success = false, message = message };
}

public class Result<T> : Result
{
    T? data { get; set; }

    public static Result<T> OK<T>(T data, string? message) =>
        new() { data = data, success = true, message = message };
}*/
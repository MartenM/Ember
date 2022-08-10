namespace Interactivity.Bot.Services;

public enum ServiceResultState
{
    Success,
    UserError,
    BotError,
}

public class ServiceResult<T> where T : Enum
{
    
    public ServiceResult(ServiceResultState state)
    {
        State = state;
    }
    public ServiceResult(ServiceResultState state, T? error)
    {
        Error = error;
        State = state;
    }

    public T? Error { get; }
    public ServiceResultState? State { get; }

    public static ServiceResult<T> Success()
    {
        return new ServiceResult<T>(ServiceResultState.Success);
    }

    public static ServiceResult<T> UserError(T reason)
    {
        return new ServiceResult<T>(ServiceResultState.UserError, reason);
    }
    
    public static ServiceResult<T> BotError(T reason)
    {
        return new ServiceResult<T>(ServiceResultState.BotError, reason);
    }
}
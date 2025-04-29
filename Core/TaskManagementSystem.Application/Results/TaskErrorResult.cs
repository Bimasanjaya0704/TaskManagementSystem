using TaskManagementSystem.Application.Enum;
using TaskManagementSystem.Domain.Enum;

namespace TaskManagementSystem.Application.Result;
public class TaskErrorResult<T> where T : class
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public TaskErrorType ErrorType { get; private set; }
    public string ErrorMessage { get; private set; }

    private TaskErrorResult(bool isSuccess, T data, TaskErrorType errorType, string errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorType = errorType;
        ErrorMessage = errorMessage;
    }

    public static TaskErrorResult<T> Success()
    {
        return new TaskErrorResult<T>(true, null, TaskErrorType.NoError, string.Empty);
    }
    
    public static TaskErrorResult<T> Success(T data)
    {
        return new TaskErrorResult<T>(true, data, TaskErrorType.NoError, string.Empty);
    }

    public static TaskErrorResult<T> Failure(TaskErrorType errorType, string errorMessage)
    {
        return new TaskErrorResult<T>(false, null, errorType, errorMessage);
    }
}

using Core.Enum;

namespace Core.Entities;

public class TaskErrorResult<T> where T : class
{
    public bool IsSuccess { get; private set; }
    public T Data { get; private set; }
    public TaskErrorType ErrorType { get; private set; }
    public string ErrorMessage { get; private set; }

    public static TaskErrorResult<T> Success()
    {
        return new TaskErrorResult<T> { IsSuccess = true, ErrorType = TaskErrorType.NoError };
    }
    public static TaskErrorResult<T> Success(T data)
    {
        return new TaskErrorResult<T> { IsSuccess = true, Data = data, ErrorType = TaskErrorType.NoError };
    }

    public static TaskErrorResult<T> Failure(TaskErrorType errorType, string errorMessage)
    {
        return new TaskErrorResult<T> { IsSuccess = false, ErrorType = errorType, ErrorMessage = errorMessage };
    }
}
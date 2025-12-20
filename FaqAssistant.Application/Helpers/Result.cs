namespace FaqAssistant.Application.Helpers;
public class Result<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public Result(bool success = false, string? message = null)
    {
        Message = message;
        Success = success;
    }

    public Result(bool success, T data, string? message = null)
    {
        Message = message;
        Data = data;
        Success = success;
    }
}
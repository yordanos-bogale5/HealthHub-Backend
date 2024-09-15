namespace HealthHub.Source.Models.Responses;

public interface IApiResponse<T>
{
  bool Success { get; set; }
  string? Message { get; set; }
  T? Data { get; set; }
}

public class ApiResponse<T> : IApiResponse<T>
{
  public bool Success { get; set; }
  public string? Message { get; set; }
  public T? Data { get; set; }

  public ApiResponse(bool success, string? message, T? data)
  {
    Success = success;
    Message = message;
    Data = data;
  }
}

public class ErrorResponse
{
  public string title { get; set; } = "";
  public string message { get; set; } = "";
  public object errors { get; set; } = new { };
}

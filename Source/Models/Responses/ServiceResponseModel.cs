namespace HealthHub.Source.Models.Responses;


public interface IServiceResponse
{
  public bool Success { get; }
  public int StatusCode { get; }
  public string? Message { get; }
}

public interface IServiceResponse<T> : IServiceResponse
{
  T? Data { get; }
}

public class ServiceResponse(bool Success, int StatusCode, string? Message = null) : IServiceResponse
{
  public bool Success { get; } = Success;
  public string? Message { get; } = Message;
  public int StatusCode { get; } = StatusCode;
}

public class ServiceResponse<T> : ServiceResponse, IServiceResponse<T>
{
  public T? Data { get; }
  public ServiceResponse(bool Success, int StatusCode, T? Data, string? Message = null) : base(Success, StatusCode, Message)
  {
    this.Data = Data;
  }
}
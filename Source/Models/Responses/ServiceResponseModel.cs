namespace HealthHub.Source.Models.Responses;

public interface IServiceResponse
{
  public bool Success { get; set; }
  public int StatusCode { get; set; }
  public string? Message { get; set; }
}

public interface IServiceResponse<T> : IServiceResponse
{
  T? Data { get; }
}

public class ServiceResponse(bool Success, int StatusCode, string? Message = null)
  : IServiceResponse
{
  public bool Success { get; set; } = Success;
  public string? Message { get; set; } = Message;
  public int StatusCode { get; set; } = StatusCode;
}

public class ServiceResponse<T> : ServiceResponse, IServiceResponse<T>
{
  public T? Data { get; set; }

  public ServiceResponse()
    : base(true, 200) { }

  public ServiceResponse(bool Success, int StatusCode, T? Data, string? Message = null)
    : base(Success, StatusCode, Message)
  {
    this.Data = Data;
  }
}

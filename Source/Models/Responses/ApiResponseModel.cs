namespace HealthHub.Source.Models.Responses;

public interface IApiResponse<T>
{
  bool Success { get; set; }
  string? Message { get; set; }
  T? Data { get; set; }
}

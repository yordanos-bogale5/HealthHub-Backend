using System.Text.Json;
using ChapaNET;
using HealthHub.Source.Config;
using HealthHub.Source.Models.Enums;
using HealthHub.Source.Services.PaymentProviders;
using Newtonsoft.Json.Linq;
using RestSharp;

public class ChapaPaymentProvider(AppConfig appConfig, ILogger<ChapaPaymentProvider> logger)
  : IPaymentProvider
{
  public PaymentProvider PaymentProvider => PaymentProvider.Chapa;

  public Task<decimal> CheckBalanceAsync(string email)
  {
    return Task.FromResult(decimal.MaxValue);
  }

  public async Task<TransferResponseInner> TransferAsync(TransferRequestDto transferRequestDto)
  {
    try
    {
      if (appConfig.ChapaSecretKey is null)
      {
        throw new Exception("Chapa Secret Key is not set");
      }

      Chapa chapa = new(appConfig.ChapaSecretKey);

      var tx_rf = Chapa.GetUniqueRef();

      var restClient = new RestClient();
      var request = new RestRequest(
        $"{appConfig.ChapaApiOrigin}/v1/transaction/initialize",
        Method.Post
      );
      request.AddHeader("Content-Type", "application/json");
      request.AddHeader("Authorization", $"Bearer {appConfig.ChapaSecretKey}");

      request.AddJsonBody(
        new
        {
          email = transferRequestDto.SenderEmail,
          amount = transferRequestDto.Amount,
          first_name = transferRequestDto.FirstName,
          last_name = transferRequestDto.LastName,
          tx_ref = tx_rf,
          currency = "ETB",
          callback_url = appConfig.ApiOrigin,
          phone_number = transferRequestDto.PhoneNumber
        }
      );

      var response = await restClient.ExecuteAsync(request);
      var data = JsonSerializer.Deserialize<JsonElement>(
        response.Content ?? "null",
        new JsonSerializerOptions { WriteIndented = true }
      );

      if (!response.IsSuccessStatusCode)
      {
        return new TransferResponseInner
        {
          IsSuccessful = false,
          Message = JToken.Parse(data.GetProperty("message").ToString() ?? "No content"),
          TransactionReference = "null"
        };
      }

      var status = data.GetProperty("status").GetString();

      return new TransferResponseInner
      {
        IsSuccessful = status == "success",
        Message = JToken.Parse(data.ToString() ?? "No content"),
        TransactionReference = tx_rf
      };
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error transferring funds");
      throw;
    }
  }
}

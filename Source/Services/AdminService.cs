using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Entities;

public class AdminService(ILogger<AdminService> logger, ApplicationContext appContext)
{
  public async Task<Admin?> CreateAdmin(CreateAdminDto createAdminDto)
  {
    try
    {
      var adminResult = await appContext.Admins.AddAsync(createAdminDto.ToAdmin());
      var admin = adminResult.Entity;

      await appContext.SaveChangesAsync();
      return admin;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Failed to Create Admin");
      throw;
    }
  }
}
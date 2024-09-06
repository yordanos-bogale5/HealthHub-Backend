using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;

namespace HealthHub.Source.Services;

public class FileService(ApplicationContext appContext, ILogger<FileService> logger)
{
  public async Task<Models.Entities.File> CreateFileAsync(CreateFileDto createFileDto)
  {
    try
    {
      var file = await appContext.Files.AddAsync(createFileDto.ToFile());
      return file.Entity;
    }
    catch (System.Exception ex)
    {
      logger.LogError($"{ex}: An error occured trying to create a file");
      throw;
    }
  }
}

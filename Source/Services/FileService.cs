using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace HealthHub.Source.Services;

public class FileService(ApplicationContext appContext, ILogger<FileService> logger)
{
  public async Task<Models.Entities.File> CreateFileAsync(CreateFileDto createFileDto)
  {
    try
    {
      var file = await appContext.Files.AddAsync(createFileDto.ToFile());
      await appContext.SaveChangesAsync();

      return file.Entity;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An error occurred while trying to create a file");
      throw;
    }
  }

  /// <summary>
  /// It creates a file given by the createFileDto, and associates that
  /// file with the assocId given as param taking into consideration the
  /// entityType
  /// </summary>
  /// <param name="createFileDto"></param>
  /// <param name="assocId"></param>
  /// <param name="entityType"></param>
  /// <returns></returns>
  public async Task<Models.Entities.File> CreateFileAsync(
    CreateFileDto createFileDto,
    Guid assocId,
    DiscriminatorTypes entityType
  )
  {
    using IDbContextTransaction transaction = await appContext.Database.BeginTransactionAsync();
    try
    {
      var file = await appContext.Files.AddAsync(createFileDto.ToFile());
      await appContext.SaveChangesAsync();

      // estabilish an association between the file and the EntityType
      await CreateFileAssociationAsync(file.Entity.FileId, assocId, entityType);

      await transaction.CommitAsync();

      return file.Entity;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();

      logger.LogError($"{ex}: An error occured trying to create a file");
      throw;
    }
  }

  public async Task<FileAssociation> CreateFileAssociationAsync(
    Guid fileId,
    Guid assocId,
    DiscriminatorTypes entityType
  )
  {
    try
    {
      FileAssociation fa = entityType switch
      {
        DiscriminatorTypes.Message
          => new MessageFileAssociation { FileId = fileId, MessageId = assocId },
        _ => throw new ArgumentException($"Unsupported Discriminator Type: {entityType}")
      };

      await appContext.FileAssociations.AddAsync(fa);
      await appContext.SaveChangesAsync();

      return fa;
    }
    catch (Exception ex)
    {
      logger.LogError($"{ex}: Error occurred while creating file association");
      throw;
    }
  }
}

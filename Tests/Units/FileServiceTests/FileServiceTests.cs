using HealthHub.Source.Data;
using HealthHub.Source.Helpers;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Xunit;

namespace HealthHub.Tests.Unit.FileServiceTests;

public class FileServiceTests
{
  [Fact]
  public async Task CreateFileAsync_ShouldReturnNewlyCreatedFile()
  {
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationContext>()
      .UseInMemoryDatabase(databaseName: "TestDatabase")
      .Options;
    var mockAppContext = new ApplicationContext(options);
    var mockLogger = new Mock<ILogger<FileService>>();
    var fileService = new FileService(mockAppContext, mockLogger.Object);

    var createFileDto = new CreateFileDto("image/jpg", "AA==", "wallpaper.jpeg");

    // Act
    var result = await fileService.CreateFileAsync(createFileDto);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("wallpaper.jpeg", result.FileName);
    Assert.Equal("image/jpg", result.MimeType);
    Assert.Equal("AA==", FileHelper.ToBase64(result.FileData));
  }
}

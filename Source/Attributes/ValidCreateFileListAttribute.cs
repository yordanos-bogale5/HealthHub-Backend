using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Helpers;
using HealthHub.Source.Models.Defaults;

/// <summary>
///
/// </summary>
public class ValidCreateFileListAttribute : ValidationAttribute
{
  int maxFileSize;
  string[]? allowedExtensions;

  public ValidCreateFileListAttribute() { }

  /// <param name="maxFileSize">if not provided limit will be 10,485,760(10MB)</param>
  /// <param name="allowedExtensions">if not provided all files will be allowed</param>
  public ValidCreateFileListAttribute(
    int maxFileSize = 10_485_760,
    string[]? allowedExtensions = null
  )
  {
    this.maxFileSize = maxFileSize;
    this.allowedExtensions = allowedExtensions;
  }

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var fileList = value as List<CreateFileDto>;

    if (fileList != null)
    {
      foreach (var file in fileList)
      {
        if (file == null)
          return new ValidationResult("File cannot be a type of null.");

        int fileSize = FileHelper.ToByteStream(file.FileDataBase64).Count();
        if (fileSize > maxFileSize)
          return new ValidationResult("File size should be at most 10Mb.");

        bool isSupportedMime = Mime.IsSupportedMimeValue(file.MimeType);
        if (!isSupportedMime)
          return new ValidationResult("Mime type for the file is not supported.");
      }
    }

    return ValidationResult.Success;
  }
}

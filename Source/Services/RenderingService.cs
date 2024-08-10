using MiniRazor;

namespace HealthHub.Source.Services;

public class RenderingService(FileService fileService)
{

  public async Task<string> RenderRazorPage(string filePath, object viewModel)
  {
    var fileContents = await fileService.ReadFile(Path.Combine(Directory.GetCurrentDirectory(), filePath));
    var template = Razor.Compile(fileContents);
    var output = await template.RenderAsync(viewModel);
    return output;
  }
}
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Services.BlogService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/blogs")]
public class BlogController(IBlogService blogService) : ControllerBase
{
  [HttpGet("all")]
  public IActionResult GetAllBlogs() => throw new NotImplementedException();

  [HttpGet("{id}")]
  public IActionResult GetBlogById([FromRoute] Guid id) => throw new NotImplementedException();

  [HttpPost]
  public async Task<IActionResult> CreateBlog([FromBody] CreateBlogDto createBlogDto)
  {
    try
    {
      if (!ModelState.IsValid)
      {
        HttpContext.Items[ErrorFieldConstants.ModelStateErrors] = ModelState;
        throw new BadHttpRequestException(ErrorMessages.ModelValidationError);
      }
      var result = await blogService.CreateBlogAsync(createBlogDto);
      return Ok(result);
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  [HttpPut("{blogId}")]
  public IActionResult UpdateBlog(
    [FromRoute] Guid blogId,
    [FromBody] CreateBlogDto createBlogDto
  ) => throw new NotImplementedException();
}

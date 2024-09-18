using System.ComponentModel.DataAnnotations;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Services.BlogService;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/blogs")]
public class BlogController(IBlogService blogService, ILogger<BlogController> logger)
  : ControllerBase
{
  [HttpGet("all")]
  public async Task<IActionResult> GetAllBlogs()
  {
    try
    {
      var result = await blogService.GetAllBlogsAsync();
      return Ok(result);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to get all the blogs.");
      throw;
    }
  }

  [HttpGet("{blogId}")]
  public async Task<IActionResult> GetBlogById([FromRoute] [Required] [Guid] Guid blogId)
  {
    try
    {
      var result = await blogService.GetBlogAsync(blogId);
      return Ok(result);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to get blog by id.");
      throw;
    }
  }

  [HttpPost]
  public async Task<IActionResult> CreateBlog([FromBody] CreateBlogDto createBlogDto)
  {
    try
    {
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

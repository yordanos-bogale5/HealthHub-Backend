using HealthHub.Source.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/blogs")]
public class BlogController() : ControllerBase
{
  [HttpGet("all")]
  public IActionResult GetAllBlogs() => throw new NotImplementedException();

  [HttpGet("{id}")]
  public IActionResult GetBlogById([FromRoute] Guid id) => throw new NotImplementedException();

  [HttpPost]
  public IActionResult CreateBlog([FromBody] CreateBlogDto createBlogDto) =>
    throw new NotImplementedException();

  [HttpPut("{blogId}")]
  public IActionResult UpdateBlog(
    [FromRoute] Guid blogId,
    [FromBody] CreateBlogDto createBlogDto
  ) => throw new NotImplementedException();
}

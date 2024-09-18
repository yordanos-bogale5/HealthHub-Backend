using System.Data;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Services;
using HealthHub.Source.Services.BlogService;
using Microsoft.EntityFrameworkCore;

public class BlogService(ApplicationContext appContext, ILogger<BlogService> logger) : IBlogService
{
  public async Task<BlogDto> CreateBlogAsync(CreateBlogDto createBlogDto)
  {
    try
    {
      // Search the doctor table if there is a user with the given userId
      var author = await appContext
        .Doctors.Include(d => d.User)
        .FirstOrDefaultAsync(d => d.UserId == createBlogDto.AuthorId);

      if (author == default)
        throw new KeyNotFoundException(
          "Author with the specified id doesn't exist. Please make sure you provided a userId of a doctor!"
        );

      if (await SlugExistsAsync(createBlogDto.Slug))
        throw new BadHttpRequestException("Slug already exists. Please choose another slug!");

      var blog = createBlogDto.ToBlog();
      var result = await appContext.AddAsync(blog);

      await appContext.SaveChangesAsync();

      return result.Entity.ToBlogDto(author.User, blog.BlogLikes);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to create a blog.");
      throw;
    }
  }

  public async Task<bool> SlugExistsAsync(string slug)
  {
    return await appContext.Blogs.FirstOrDefaultAsync(b => b.Slug == slug) != default;
  }

  public async Task<List<BlogDto>> GetAllBlogsAsync()
  {
    try
    {
      var blogs = await appContext
        .Blogs.Include(b => b.Author)
        .Include(b => b.BlogLikes)
        .Where(b => b.Author != null)
        .Select(b => b.ToBlogDto(b.Author!, b.BlogLikes))
        .ToListAsync();
      return blogs;
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to get all blogs");
      throw;
    }
  }

  public async Task<BlogDto> GetBlogAsync(Guid blogId)
  {
    try
    {
      var blog = await appContext
        .Blogs.Include(b => b.Author)
        .Include(b => b.BlogLikes)
        .FirstOrDefaultAsync(b => b.BlogId == blogId);

      if (blog == default)
        throw new KeyNotFoundException("Blog with the given id is not found!");

      return blog.ToBlogDto(blog.Author!, blog.BlogLikes);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to get blog by id.");
      throw;
    }
  }

  public Task<BlogDto> UpdateBlogAsync(Guid blogId, CreateBlogDto createBlogDto) =>
    throw new NotImplementedException();
}

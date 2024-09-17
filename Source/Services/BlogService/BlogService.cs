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

      if (await SlugExists(createBlogDto.Slug))
        throw new BadHttpRequestException("Slug already exists. Please choose another slug!");

      var blog = createBlogDto.ToBlog();
      var result = await appContext.AddAsync(blog);

      await appContext.SaveChangesAsync();

      return result.Entity.ToBlogDto(author.User, blog.BlogLikes);
    }
    catch (System.Exception ex)
    {
      throw;
    }
  }

  public async Task<bool> SlugExists(string slug)
  {
    return await appContext.Blogs.FirstOrDefaultAsync(b => b.Slug == slug) != default;
  }

  public Task<List<BlogDto>> GetAllBlogsAsync() => throw new NotImplementedException();

  public Task<BlogDto> GetBlog(Guid blogId) => throw new NotImplementedException();

  public Task<BlogDto> UpdateBlog(Guid blogId, CreateBlogDto createBlogDto) =>
    throw new NotImplementedException();
}

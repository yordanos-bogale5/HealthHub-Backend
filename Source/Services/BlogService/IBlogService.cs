using HealthHub.Source.Models.Dtos;

namespace HealthHub.Source.Services.BlogService;

public interface IBlogService
{
  Task<BlogDto> CreateBlogAsync(CreateBlogDto createBlogDto);
  Task<List<BlogDto>> GetAllBlogsAsync();
  Task<BlogDto> GetBlog(Guid blogId);
  Task<BlogDto> UpdateBlog(Guid blogId, CreateBlogDto createBlogDto);
  Task<bool> SlugExists(string slug);
}

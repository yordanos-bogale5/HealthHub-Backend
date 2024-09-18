using HealthHub.Source.Models.Dtos;

namespace HealthHub.Source.Services.BlogService;

public interface IBlogService
{
  Task<BlogDto> CreateBlogAsync(CreateBlogDto createBlogDto);
  Task<List<BlogDto>> GetAllBlogsAsync();
  Task<BlogDto> GetBlogAsync(Guid blogId);
  Task<BlogDto> UpdateBlogAsync(Guid blogId, CreateBlogDto createBlogDto);
  Task<bool> SlugExistsAsync(string slug);
}

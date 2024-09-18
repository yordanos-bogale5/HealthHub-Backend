using System.Data;
using Flurl.Util;
using HealthHub.Source.Data;
using HealthHub.Source.Helpers.Extensions;
using HealthHub.Source.Models.Dtos;
using HealthHub.Source.Models.Entities;
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

      // Create the tags
      var tags = await CreateTagsAsync(createBlogDto.Tags);

      var blog = createBlogDto.ToBlog();

      var result = await appContext.AddAsync(blog);
      await appContext.SaveChangesAsync();

      // Estabilish association between blog and a tag
      await CreateBlogTagAssocAsync(result.Entity.BlogId, tags);

      return result.Entity.ToBlogDto(author.User, result.Entity.BlogLikes, tags);
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
        .Include(b => b.BlogTags)
        .ThenInclude(bt => bt.Tag)
        .Where(b => b.Author != null)
        .Select(b => b.ToBlogDto(b.Author!, b.BlogLikes, b.BlogTags.Select(bt => bt.Tag!).ToList()))
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
        .Include(b => b.BlogTags)
        .ThenInclude(bt => bt.Tag)
        .FirstOrDefaultAsync(b => b.BlogId == blogId);

      if (blog == default)
        throw new KeyNotFoundException("Blog with the given id is not found!");

      return blog.ToBlogDto(
        blog.Author!,
        blog.BlogLikes,
        blog.BlogTags.Select(bt => bt.Tag!).ToList()
      );
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to get blog by id.");
      throw;
    }
  }

  public async Task<BlogDto> UpdateBlogAsync(Guid blogId, EditBlogDto editBlogDto)
  {
    try
    {
      var blog = await appContext
        .Blogs.Include(b => b.Author)
        .Where(b => b.Author != null)
        .Include(b => b.BlogLikes)
        .FirstOrDefaultAsync(b => b.BlogId == blogId);

      if (blog == default)
        throw new KeyNotFoundException("Blog with the given id doesn't exist. Unable to update.");

      blog.Title = editBlogDto.Title;
      blog.Slug = editBlogDto.Slug;
      blog.Summary = editBlogDto.Summary;
      blog.Content = editBlogDto.Content;

      var tags = await CreateTagsAsync(editBlogDto.Tags);

      // Estabilish association between blog and a tag
      await CreateBlogTagAssocAsync(blog.BlogId, tags);

      await appContext.SaveChangesAsync();
      return blog.ToBlogDto(blog.Author!, blog.BlogLikes, tags);
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to update blog.");
      throw;
    }
  }

  public async Task<bool> BlogExistsAsync(Guid blogId)
  {
    return await appContext.Blogs.FirstOrDefaultAsync(b => b.BlogId == blogId) != default;
  }

  public async Task<ICollection<Tag>> CreateTagsAsync(IList<string> tags)
  {
    try
    {
      ICollection<Tag> result = [];

      var existingTags = await appContext.Tags.Where(t => tags.Contains(t.TagName)).ToListAsync();

      foreach (var tag in tags)
      {
        // Search the db for such a tag
        var tagInDb = existingTags.FirstOrDefault(t => t.TagName == tag);

        if (tagInDb != default)
        {
          // if the tag exists no need to create just use it
          result.Add(tagInDb);
        }
        else
        {
          // if there is no such tag then create a new one
          var createdTag = await appContext.Tags.AddAsync(new Tag { TagName = tag });
          result.Add(createdTag.Entity);
        }
      }

      await appContext.SaveChangesAsync();
      return result;
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occured trying to create tags.");
      throw;
    }
  }

  public async Task<ICollection<BlogTag>> CreateBlogTagAssocAsync(
    Guid blogId,
    ICollection<Tag> tags
  )
  {
    try
    {
      var blog = await appContext
        .Blogs.Include(b => b.BlogTags)
        .FirstOrDefaultAsync(b => b.BlogId == blogId);

      if (blog == default)
        throw new KeyNotFoundException(
          "Blog with the given id doesn't exist. Unable to create blog tag association."
        );

      // Fetch existing blog-tag associations for the given blogId
      var existingBlogTags = appContext
        .BlogTags.Where(bt => bt.BlogId == blogId)
        .Select(bt => bt.TagId)
        .ToHashSet();

      var blogTagsCreated = new List<BlogTag>();

      foreach (var tag in tags)
      {
        if (!existingBlogTags.Contains(tag.TagId))
        {
          // Add the association to the collection if it doesn't exist
          blogTagsCreated.Add(new BlogTag { BlogId = blogId, TagId = tag.TagId });
        }
      }

      if (blogTagsCreated.Any())
      {
        await appContext.BlogTags.AddRangeAsync(blogTagsCreated);
        await appContext.SaveChangesAsync();
      }

      return await appContext.BlogTags.Where(bt => bt.BlogId == blog.BlogId).ToListAsync();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "An error occurred trying to create blog tag association.");
      throw;
    }
  }

  public void DeleteAllBlogs()
  {
    try
    {
      appContext.Blogs.RemoveRange(appContext.Blogs);
      appContext.SaveChanges();
    }
    catch (System.Exception ex)
    {
      logger.LogError(ex, "Error deleting all blogs");
      throw;
    }
  }
}

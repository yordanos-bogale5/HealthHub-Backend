using HealthHub.Source.Models.Entities;

public class Tag : BaseEntity
{
  public Guid TagId { get; set; }
  public required string TagName { get; set; }
  public ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
}

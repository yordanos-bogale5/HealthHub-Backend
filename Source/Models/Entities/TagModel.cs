using HealthHub.Source.Models.Entities;

public class Tag : BaseEntity
{
  public Guid TagId { get; set; }
  public required string TagName { get; set; }
  public virtual ICollection<BlogTag> BlogTags { get; set; } = new HashSet<BlogTag>();
}

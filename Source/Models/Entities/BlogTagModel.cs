namespace HealthHub.Source.Models.Entities;

public class BlogTag
{
  public Guid BlogId { get; set; }
  public Guid TagId { get; set; }
  public virtual Blog? Blog { get; set; }
  public virtual Tag? Tag { get; set; }
}

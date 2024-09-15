using System.Net.Http.Headers;
using HealthHub.Source.Helpers.Defaults;

namespace HealthHub.Source.Models.Entities;

public abstract class FileAssociation
{
  public Guid FileAssociationId { get; set; }

  public Guid FileId { get; set; }
  public virtual File? File { get; set; }
}

public class MessageFileAssociation : FileAssociation
{
  public Guid MessageId { get; set; }
  public virtual Message? Message { get; set; }
}

// public class BlogCommentFileAssociation : FileAssociation
// {
//   public Guid BlogCommentId { get; set; }
//   public virtual BlogComment? BlogComment { get; set; }
// }

// public class DocumentFileAssociation : FileAssociation
// {
//   public override DiscriminatorTypes EntityType => DiscriminatorTypes.Document;

//   public Guid DocumentId { get; set; }
//   public virtual BlogComment? BlogComment { get; set; }
// }

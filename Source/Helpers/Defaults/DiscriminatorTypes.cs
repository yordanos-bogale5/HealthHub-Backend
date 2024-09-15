namespace HealthHub.Source.Helpers.Defaults;

// We use these for discriminating between different
// tables when defining our models. For example fileAssociation
// model will be specialized into different tables like messageAssoc,
// documentAssoc etc. Hence these types will be used as values for
// the discriminant column
public enum DiscriminatorTypes
{
  Message,
  BlogComment,
  Document,
}

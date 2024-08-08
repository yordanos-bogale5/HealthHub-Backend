namespace HealthHub.Source.Attributes;

public class AgeAbove18Attribute : AgeAboveAttributes
{
  public AgeAbove18Attribute() : base(18)
  {
  }

  public override string FormatErrorMessage(string name)
  {
    return $"You must be at least 18 years old.";
  }
}
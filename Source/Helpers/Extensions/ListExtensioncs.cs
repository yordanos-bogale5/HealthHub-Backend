using HealthHub.Source.Models.Dtos;

namespace HealthHub.Source.Helpers.Extensions;

public static class ListExtensions
{
    public static List<CreateSpecialityDto> ToSpecialityList(
        this List<string> strings,
        Guid doctorId
    )
    {
        return strings.Select(str => new CreateSpecialityDto { SpecialityName = str }).ToList();
    }
}

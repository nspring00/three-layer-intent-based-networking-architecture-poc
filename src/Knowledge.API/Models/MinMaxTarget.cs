namespace Knowledge.API.Models;

public record MinMaxTarget(float? Min, float? Max)
{
    public bool HasMin => Min.HasValue;
    public bool HasMax => Max.HasValue;
}

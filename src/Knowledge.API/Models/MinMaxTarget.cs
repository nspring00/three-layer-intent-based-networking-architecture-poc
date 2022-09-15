namespace Knowledge.API.Models;

public record MinMaxTarget(float? Min, float? Max)
{
    public bool HasMin => Min.HasValue;
    public bool HasMax => Max.HasValue;
    public bool IsInRange(float value)
    {
        if (HasMin && value < Min)
            return false;
        if (HasMax && value > Max)
            return false;
        return true;
    }
}

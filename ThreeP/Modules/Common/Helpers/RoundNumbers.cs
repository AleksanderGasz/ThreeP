namespace Mac.Modules.Common;

public static class RoundNumbers
{
    public static float Round(this float? value, int decimalPlaces = 2)
    {
        if (!value.HasValue) return 0;
        return (float)MathF.Round(value.Value, decimalPlaces);
        
    }
}
namespace Mac.Modules.Common;

public static class ColorMixer
{
    public static string? GetRandomColor()
    {
        var random = new Random();
        var r = random.Next(0, 255);
        var g = random.Next(0, 255);
        var b = random.Next(0, 255);
        return $"#{r:X2}{g:X2}{b:X2}";
    }


    public static string? GetRandomColorRgba(float alpha = 1.0f)
    {
        var random = new Random();
        var r = random.Next(0, 256);
        var g = random.Next(0, 256);
        var b = random.Next(0, 256);
        // var a = alpha;
        var alphaValue = Math.Clamp(alpha, 0.0f, 1.0f);
        return $"rgba({r},{g},{b},{alphaValue.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)})";
    }
}
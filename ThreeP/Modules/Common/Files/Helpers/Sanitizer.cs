namespace Mac.Modules.Files;

public static class Sanitizer
{
    public static string? Sanitize(this string? input)
    {
        if( string.IsNullOrWhiteSpace(input ))return string.Empty;
        
        var invalidChars=Path.GetInvalidFileNameChars();
        var sanitized = input.Trim();
        foreach (var invalidChar in invalidChars)
        {
            sanitized = sanitized.Replace(invalidChar, '-').Replace(' ','-');
        }

        return sanitized;
    }
}
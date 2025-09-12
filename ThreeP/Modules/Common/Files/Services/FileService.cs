namespace Mac.Modules.Files;

public class FileService(IWebHostEnvironment environment)
{
    public (string? filePath, string? dowloadedPath) SetFilePath(string fileName, string? directoryPath = null)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return new();

        if (string.IsNullOrWhiteSpace(directoryPath))
            directoryPath = Path.Combine(environment.WebRootPath, "media", "files");
        else directoryPath = Path.Combine(environment.WebRootPath, directoryPath);

        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, fileName);
        var dowloadedPath = Path.GetRelativePath(environment.WebRootPath, filePath);

        return (filePath, dowloadedPath);

        // if (File.Exists(filePath)) File.Delete(filePath);
    }
}
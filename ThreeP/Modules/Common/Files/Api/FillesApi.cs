namespace ThreeP.Modules.Files;

public static class FillesApi
{
    public static void AddGetFileEndpoint(this WebApplication app)
    {
        app.MapGet("/api/download/files/notices/{fileName}", async (string fileName, FileService fileService) =>
        {
            try
            {
                var (filePath, _) = fileService.SetFilePath(fileName);
                if (!File.Exists(filePath)) return Results.NotFound("Brak pliku");

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                return Results.File(fileBytes, "application/zip", fileName);
            }
            catch (Exception e)
            {
                return Results.Problem($"Błąd podczas pobierania pliku: {e.Message}");
            }
        }).WithName("DownloadNotices").WithTags("Download");
    }
}
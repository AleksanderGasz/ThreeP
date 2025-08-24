namespace Mac.Modules.Files;

public class ZipService(FileService fileService)
{
    public async Task<(string? filePath, string? dowloadedPath)> CreateZipFromFiles(HashSet<string?> filePatches,
        string? zipName, string? zipDirectory = null)
    {
        // SPRAWDZENIE PARAMETRÓW WEJŚCIOWYCH
        if (filePatches is { Count: 0 } || string.IsNullOrWhiteSpace(zipName)) return (string.Empty, string.Empty);

        // USTAWIENIE ŚCIEŻKI ZIP
        var (zipPath, downloadedPath) = fileService.SetFilePath(zipName, zipDirectory);

        // USUWANIE  PLIKU ZIP JEŻELI JUŻ ISTNIEJE
        if (File.Exists(zipPath)) File.Delete(zipPath);

        try
        {
            // TWORZENIE STREAM DLA PLIKU ZIP
            await using var stream = new FileStream(zipPath, FileMode.Create, FileAccess.Write);

            // TWORZENIE ARCHIWUM ZIP
            await using var zipArchive =
                await ZipArchive.CreateAsync(stream, ZipArchiveMode.Create, false, Encoding.UTF8);

            // DODAWANIE PLIKÓW DO ARCHIWUM ZIP
            foreach (var filePatch in filePatches)
            {
                if (!File.Exists(filePatch)) continue;

                var fileName = Path.GetFileName(filePatch);
                var entry = zipArchive.CreateEntry(fileName, compressionLevel: CompressionLevel.Optimal);

                await using var entryStream = entry.Open();
                await using var sourceFileStream = File.OpenRead(filePatch);
                await sourceFileStream.CopyToAsync(entryStream);

                await entryStream.FlushAsync();
            }

            await stream.FlushAsync();

            return (zipPath, downloadedPath);
        }
        catch (Exception e)
        {
            if(File.Exists(zipPath)) File.Delete(zipPath);
            throw new InvalidOperationException($"Błąd podczas tworzenia archiwum ZIP: {e.Message}", e);
        }
    }
}
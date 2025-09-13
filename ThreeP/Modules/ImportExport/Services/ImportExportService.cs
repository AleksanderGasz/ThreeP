namespace ThreeP.Modules.ImportExport;

public class ImportExportService<T>(FileService fileService, IWebHostEnvironment environment, ILogger<T> logger)
{
    public async Task<Result> Export(HashSet<T>? items, CancellationToken cancel = default)
    {
        if (items is null or { Count: 0 }) return Result.Fail(LogText.ObjectIsNull);

        await using var fileStream = File.Create(Path.Combine(environment.WebRootPath, "media", "files", "export",
            $"{typeof(T).Name.ToLowerInvariant()}.json"));
        var jsonOptions = new JsonSerializerOptions
            { ReferenceHandler = ReferenceHandler.IgnoreCycles, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,WriteIndented = true };
        try
        {
            await JsonSerializer.SerializeAsync<HashSet<T>>(fileStream, items, jsonOptions, cancel);
            return Result.Ok().WithSuccess(LogText.FileSaveOk);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Result.Fail($"{LogText.CantSaveFile} : {e.Message}");
        }
    }


    public async Task<Result<HashSet<T>>> Import(CancellationToken cancel = default)
    {
        var path = Path.Combine(Path.Combine(environment.WebRootPath, "media", "files", "export",
            $"{typeof(T).Name.ToLowerInvariant()}.json"));
        if (!File.Exists(path)) return Result.Fail(LogText.FileNotExist);
        try
        {
            await using var fileStream = File.OpenRead(path);
            var result = await JsonSerializer.DeserializeAsync<HashSet<T>>(fileStream, cancellationToken: cancel) ?? [];
            return result.Count > 0
                ? Result.Ok(result).WithSuccess(LogText.FileImportedOk)
                : Result.Fail(LogText.FileImportFail);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"{LogText.FileImportFail}: {e.Message} ");
            return Result.Fail(LogText.FileImportFail);
            
        }
    }
}
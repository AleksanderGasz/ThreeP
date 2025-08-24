namespace Mac.Modules.Pdf;

public class PdfService(FileService fileService)
{
    public async Task<(string? filePath, string? dowloadedPath)> GeneratePdf(string html, string name)
    {
        if (string.IsNullOrEmpty(html) || string.IsNullOrEmpty(name)) return new();

        // PDF
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);
        var directoryPath = Path.Combine("media", "files", "bzp-pdfs");

        var paths = fileService.SetFilePath(name, directoryPath);
        await page.PdfAsync(paths.filePath);
        if (!File.Exists(paths.filePath)) return new();
        return paths;
    }
}
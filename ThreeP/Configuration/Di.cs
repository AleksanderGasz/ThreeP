namespace ThreeP.Configuration;

public static class Di
{
    // extension(IServiceCollection services)
    // {
    // public IServiceCollection AddServices(IConfiguration configuration, IWebHostEnvironment environment)
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {

        
        // 3RD PARTY
        services.AddMudServices().AddSnackbarConfig();
        MudGlobal.UnhandledExceptionHandler = (exception) => Console.WriteLine(exception);


        //COMMON  SERVICES
        services.AddScoped<AppState>();
        services.AddScoped<Seed>();
        services.AddScoped(typeof(GenericService<>));
        services.AddScoped(typeof(GenericServiceWithUser<>));
        services.AddScoped<SettingsService>();
        services.AddScoped<ProgressService>();
        services.AddScoped<ShowResultService>();
        services.AddScoped<FileService>();
        services.AddScoped<ZipService>();
        services.AddScoped<PdfService>();
        
        // APP SERVICES

        
        return services;
    }
    // }
}
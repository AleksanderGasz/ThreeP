using Serilog.Sinks.SystemConsole.Themes;

namespace ThreeP.Configuration;

public static class SerilogConfig
{
    public static void Startup()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .WithConsole()
            .WithAspireOpenTelemetry("ThreeP")
            .CreateBootstrapLogger();
    }


    public static void Configure(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();

        builder.Host.UseSerilog((context, services, configuration) =>
            configuration
                .ReadFrom.Configuration(context.Configuration)
                /*.MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore.Components.Server.Circuits", LogEventLevel.Warning)*/
                .Enrich.FromLogContext()
                .WithConsole()
                .WithAspireOpenTelemetry(builder.Environment.ApplicationName)
        );
    }
}

public static class SerilogExtensions
{
    public static LoggerConfiguration WithConsole(this LoggerConfiguration configuration)
    {
        configuration.WriteTo.Console(
            theme: AnsiConsoleTheme.Code,
            applyThemeToRedirectedOutput: true,
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}");
        // "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext:l} {Message:lj} {Properties:j}{NewLine}{Exception}"));    
        return configuration;
    }


    public static LoggerConfiguration WithAspireOpenTelemetry(this LoggerConfiguration configuration, string appName,
        string? endpoint = "http://localhost:4317")
    {
        configuration.WriteTo.OpenTelemetry(config =>
        {
            config.Endpoint = "http://localhost:4317";
            config.Protocol = OtlpProtocol.Grpc;
            config.ResourceAttributes = new Dictionary<string, object>()
            {
                ["service.name"] = appName
            };
        });
        return configuration;
    }
}
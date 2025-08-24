namespace ThreeP.Configuration;

public static class Options
{
    public static IServiceCollection AddAppOptions(this IServiceCollection services, IConfiguration configuration)
    {
        /*services.AddOptions<BzpOptions>().Bind(configuration.GetSection("Bzp"))
            .ValidateDataAnnotations()
            .ValidateOnStart();*/
        return services;
    }
}
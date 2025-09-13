namespace ThreeP.Configuration;

public static class Configuration
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // CONFIGURATION
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        // HTTP
        // services.AddBzpHttpClient();

        // IDENTITY
        services.AddCascadingAuthenticationState();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 0;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        // COOKIES
        services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromDays(100);
            options.SlidingExpiration = true;
        });

        // DB
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException(
                                   "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(connectionString);
                //!ATTENTION ERRORS LOGIN
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        );
        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString), ServiceLifetime.Scoped);


        services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// JSON
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNameCaseInsensitive =true;
            options.ReferenceHandler = ReferenceHandler.Preserve;
            options.WriteIndented = true;
        });
        
        // JSON - Konfiguracja dla HTTP API
        /*services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; // Obsługa cyklicznych referencji
            options.SerializerOptions.WriteIndented = true;
        });*/

        // JSON - Singleton dla bezpośredniego użycia w serwisach
        /*services.AddSingleton(provider => new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = ReferenceHandler.Preserve, // Obsługa cyklicznych referencji Items ↔ Sets
            WriteIndented = environment.IsDevelopment()
        });*/

        // IOPTIONS
        services.AddAppOptions(configuration);

        //DATA PROTECTION
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
                .SetApplicationName(environment.ApplicationName);
        }
        
        // ASPIRE
        

        return services;
    }
}
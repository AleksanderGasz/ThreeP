IConfiguration configuration;

// SerilogConfig.Startup();

// try
// {


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// LOGGER
// builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.ColorBehavior = LoggerColorBehavior.Enabled;
    options.IncludeScopes = true;
});
StaticLogger.Log = builder.Logging.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
StaticLogger.Log.LogInformation("App STARTING UP!!");


// JSONS
builder.Configuration.AddJsonFile(Path.Combine("Configuration", "Json", "appsettings.json"), optional: false,
    reloadOnChange: true);
builder.Configuration.AddJsonFile(
    Path.Combine("Configuration", "Json", $"appsettings.{builder.Environment.EnvironmentName}.json"),
    optional: true, reloadOnChange: true);


configuration = builder.Configuration;

builder.Services.AddConfiguration(configuration, builder.Environment);
builder.Services.AddServices(configuration, builder.Environment);
// SerilogConfig.Configure(builder);

var app = builder.Build();

// app.Logger.LogInformation("APP STARTING UP!!");

app.MapDefaultEndpoints();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForErrors: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();


// DB ESTABLISH
var scope = app.Services.CreateAsyncScope();
var seed = scope.ServiceProvider.GetRequiredService<Seed>();

if (configuration.GetValue<bool>("DbSetup"))
{
    seed.EnsureDataDirectoryExist();
    await seed.EstablishDb();
    await seed.AddAdmin("maciek@kukuczka.net", "mac101");
}

//MINIMAL API
app.AddGetFileEndpoint();

app.Run();
app.Logger.LogInformation("APP STARTED!!");

// Log.Information("APP STARTED!!");

/*}
catch (Exception e)
{
    Log.Fatal(e,"""
              App CRUSHED!!
               {0}
              """, e.Message);
}
finally
{
    Log.CloseAndFlush();
}*/
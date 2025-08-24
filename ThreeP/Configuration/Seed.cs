namespace ThreeP.Configuration;

public class Seed(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
{
    public async Task<bool> IsDbExist()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        try
        {
            return await dbContext.Database.CanConnectAsync();
        }
        catch (Exception e)
        {
            Log.Error($"BŁĄD PODCZAS SPRAWDZANIA BAZY DANYCH {e.Message}");
            return false;
        }
    }


    public void EnsureDataDirectoryExist()
    {
        string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }

        Log.Information($"Ścieżka do Bazy Danych {dataDirectory}");
    }


    public async Task EstablishDb()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        try
        {
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            Log.Error($"BŁĄD PODCZAS MIGRACJI BAZY DANYCH {e.Message}");
        }
        /*await dbContext.Database.EnsureCreatedAsync();
        if (await dbContext.Database.CanConnectAsync())
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (pendingMigrations is not null && pendingMigrations.Any())
            {
                await dbContext.Database.MigrateAsync();
            }
        }*/
    }


    public async Task AddAdmin(string? email, string? password)
    {
        try
        {
            var result = await userManager.FindByEmailAsync(email);
            if (result is not null) return;
            var user = new ApplicationUser
            {
                Id = Guid.CreateVersion7(),
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.CreateVersion7().ToString(),
                SecurityStamp = Guid.CreateVersion7().ToString()
            };

            var createUserResult = await userManager.CreateAsync(user, password);
            if (!createUserResult.Succeeded)
            {
                Log.Logger.Error(
                    $"BŁĄD PODCZAS TWORZENIA ADMINA: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}");
                return;
            }

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
            }

            await userManager.AddToRoleAsync(user, "Admin");
        }
        catch (Exception e)
        {
            Log.Logger.Error($"BŁĄD PODCZAS TWORZENIA ADMINA {e.Message} {e.InnerException.Message}");
        }
    }
}
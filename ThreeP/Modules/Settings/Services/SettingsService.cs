namespace Mac.Modules.UserSettings;

public class SettingsService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<Settings?> GetUserSettings(string? userName)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users
            .Include(x => x.Settings)
            .FirstOrDefaultAsync(x => x.UserName == userName);
        if (user is not null) return user.Settings;
        return null;
    }


    public async Task UpdateUserSettings(Settings settings)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == settings.UserId);
        if (user is not null)
        {
            var exist = await dbContext.Settings
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (exist is null) await dbContext.Settings.AddAsync(settings);
            else
            {
                settings.Id = exist.Id;
                dbContext.Entry(exist).CurrentValues.SetValues(settings);
                dbContext.Entry(exist).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
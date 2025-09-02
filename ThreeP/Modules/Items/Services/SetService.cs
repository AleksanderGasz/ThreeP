namespace ThreeP.Modules.Items;

public class SetService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ILogger<Set> logger)
    : GenericService<Set>(dbContextFactory)
{
    public async Task<Result> UpdateSet(Set? incoming)
    {
        if (incoming is null) return Result.Fail([LogText.ObjectIsNull]);
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();

            var targetIds = incoming.Items?.Select(x => x.Id).ToHashSet();
            var exist = await dbContext.Sets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == incoming.Id);

            if (exist is null)
            {
                exist = new Set
                {
                    Id = incoming.Id,
                    Name = incoming.Name,
                    Description = incoming.Description,
                    Weight = incoming.Weight,
                    Items = []
                };
                await dbContext.Sets.AddAsync(exist);
            }
            else
            {
                exist.Name = incoming.Name;
                exist.Description = incoming.Description;
                exist.Weight = incoming.Weight;
                exist.Items.Clear();
            }

            if (targetIds.Count > 0)
            {
                var items = await dbContext.Items.Where(x => targetIds.Contains(x.Id)).ToHashSetAsync();

                foreach (var item in items) exist.Items.Add(item);
            }

            var saved = await dbContext.SaveChangesAsync()>0;

            return saved ? Result.Ok().WithSuccess(LogText.ObjectSaved) : Result.Fail(LogText.CantSave);
        }
        catch (Exception e)
        {
            logger.LogError(e, "{LogText}, Id: {Id}", LogText.ExceptionOccurred, incoming.Id);
            return Result.Fail([LogText.ExceptionOccurred, e.Message]);
        }
    }
}
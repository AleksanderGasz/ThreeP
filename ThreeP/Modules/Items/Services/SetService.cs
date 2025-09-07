namespace ThreeP.Modules.Items;

public class SetService(IDbContextFactory<ApplicationDbContext> dbDbFactory, ILoggerFactory loggerFactory)
    : GenericService<Set>(dbDbFactory, loggerFactory)
{
    ILogger<Set> log => loggerFactory.CreateLogger<Set>();


    /*
    public async Task<Result> UpsertSet(Set? incoming, CancellationToken cancel = default)
    {
        if (incoming is null) return Result.Fail(LogText.ObjectIsNull);
        var itemsIds = incoming.Items.Select(x => x.Id).ToHashSet();
        var tripsIds = incoming.Trips.Select(x => x.Id).ToHashSet();

        try
        {
            await using var db = await dbDbFactory.CreateDbContextAsync();


            return await Upsert(incoming, (src, dst) =>
            {
                dst.Name = src.Name;
                dst.Description = src.Description;
                dst.Weight = src.Weight;

                dst.UserId = src.UserId;
                dst.Items.Clear();
                dst.Trips.Clear();

                if (itemsIds.Any())
                {
                    var items = db.Items.Where(x => itemsIds.Contains(x.Id));
                    foreach (var item in items) dst.Items.Add(item);
                }
            }, cancel);
        }
        catch (Exception e)
        {
            log.LogError(e, "{LogText}, Id: {Id}", LogText.Exception, incoming.Id);
            return Result.Fail([LogText.Exception, e.Message]);
        }
    }
    */


    public async Task<Result> UpdateSet(Set? incoming, CancellationToken cancel = default)
    {
        if (incoming is null) return Result.Fail([LogText.ObjectIsNull]);
        try
        {
            await using var dbContext = await dbDbFactory.CreateDbContextAsync(cancel);

            var targetIds = incoming.Items?.Select(x => x.Id).ToHashSet();
            var exist = await dbContext.Sets.Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == incoming.Id, cancel);

            if (exist is null)
            {
                exist = new Set
                {
                    Id = incoming.Id,
                    Name = incoming.Name,
                    Description = incoming.Description,
                    Weight = incoming.Weight,
                    Items = [],
                    UserId = incoming.UserId
                };
                await dbContext.Sets.AddAsync(exist);
            }
            else
            {
                exist.Name = incoming.Name;
                exist.Description = incoming.Description;
                exist.Weight = incoming.Weight;
                exist.Items.Clear();
                exist.UserId = incoming.UserId;
            }

            if (targetIds.Count > 0)
            {
                var items = await dbContext.Items.Where(x => targetIds.Contains(x.Id)).ToHashSetAsync();
                foreach (var item in items) exist.Items.Add(item);
            }

            var saved = await dbContext.SaveChangesAsync() > 0;

            return saved ? Result.Ok().WithSuccess(LogText.SaveOk) : Result.Fail(LogText.SaveFail);
        }
        catch (Exception e)
        {
            log.LogError(e, "{LogText}, Id: {Id}", LogText.Exception, incoming.Id);
            return Result.Fail([LogText.Exception, e.Message]);
        }
    }
}
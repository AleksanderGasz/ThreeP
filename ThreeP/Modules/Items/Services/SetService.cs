namespace ThreeP.Modules.Items;

public class SetService(IDbContextFactory<ApplicationDbContext> dbDbFactory, ILoggerFactory loggerFactory)
    : GenericService<Set>(dbDbFactory, loggerFactory)
{
    ILogger<Set> log => loggerFactory.CreateLogger<Set>();


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
                await dbContext.Sets.AddAsync(exist, cancel);
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
                var items = await dbContext.Items.Where(x => targetIds.Contains(x.Id)).ToHashSetAsync(cancel);
                foreach (var item in items) exist.Items.Add(item);
            }

            var saved = await dbContext.SaveChangesAsync(cancel) > 0;

            return saved ? Result.Ok().WithSuccess(LogText.SaveOk) : Result.Fail(LogText.SaveFail);
        }
        catch (Exception e)
        {
            log.LogError(e, "{LogText}, Id: {Id}", LogText.Exception, incoming.Id);
            return Result.Fail([LogText.Exception, e.Message]);
        }
    }


    public async Task<Result> UpsertSet(Set? incoming, CancellationToken cancel = default)
    {
        if (incoming is null) return Result.Fail(LogText.ObjectIsNull);
        await using var db = await dbDbFactory.CreateDbContextAsync(cancel);

        // var exist = await db.Sets.FirstOrDefaultAsync(x => x.Id == incoming.Id && x.UserId == incoming.UserId);
        var isNew = await db.Sets.AsNoTracking()
            .AnyAsync(x => x.Id == incoming.Id && x.UserId == incoming.UserId, cancel);
        //Get all EXISTING Items from DB(Tracked)
        //Iterate by EXISTING
        //If exist in INCOMING Item => REMOVE from INCOMING and REPLACE with  EXISTING
        var existsItems = await db.Items.ToListAsync(cancel);

        if (!isNew)
        {
            existsItems.ForEach(e =>
            {
                if (incoming.Items.Any(i => i.Id == e.Id))
                {
                    var untracked = incoming.Items.FirstOrDefault(i => i.Id == e.Id);
                    incoming.Items.Remove(untracked);
                    incoming.Items.Add(e);
                }
            });
            /*foreach ( var e  in exists)
            {
                if (incoming.Items.Any(i=>i.Id==e.Id))
                {
                    var utracked = incoming.Items.FirstOrDefault(i => i.Id == e.Id);
                    incoming.Items.Remove(utracked);
                    incoming.Items.Add(e);
                }
            }*/
            await db.Sets.AddAsync(incoming, cancel);
        }
        else
        {
            var existSet = await db.Sets.Include(x => x.Items)
                .FirstOrDefaultAsync(e => e.Id == incoming.Id && e.UserId == incoming.UserId, cancel);

            existSet.Name = incoming.Name;
            existSet.Description = incoming.Description;
            existSet.Weight = incoming.Weight;

            existsItems.ForEach(e =>
            {
                if (incoming.Items.Any(i => i.Id ==e.Id)&& !existSet.Items.Any(es=>es.Id==e.Id))
                {
                    existSet.Items.Add(e);
                }
            });

            db.Sets.Update(existSet);
        }

        var saved = await db.SaveChangesAsync(cancel) > 0;
        return saved ? Result.Ok().WithSuccess(LogText.SaveOk) : Result.Fail(LogText.SaveFail);
    }


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
}
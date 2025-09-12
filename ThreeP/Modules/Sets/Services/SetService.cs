namespace ThreeP.Modules.Sets;

public class SetService(IDbContextFactory<ApplicationDbContext> dbFactory, ILoggerFactory loggerFactory)
    : GenericService<Set>(dbFactory, loggerFactory)
{
    ILogger<Set> log => loggerFactory.CreateLogger<Set>();


    public async Task<Result> UpdateSet(Set? incomingSet, CancellationToken cancel = default)
    {
        if (incomingSet is null) return Result.Fail([LogText.ObjectIsNull]);
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancel);

            var existSet = await db.Sets
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == incomingSet.Id && x.UserId == incomingSet.UserId, cancel);

            var incomingItemsIds = incomingSet.Items?.Select(x => x.Id).ToHashSet() ?? [];

            var existItemsById = incomingItemsIds.Count > 0
                ? await db.Items.ToDictionaryAsync(x => x.Id, cancel)
                : new Dictionary<Guid, Item>();


            if (existSet is null)
            {
                // ADD
                var newSet = new Set
                {
                    Id = incomingSet.Id,
                    Items = [],
                    UserId = incomingSet.UserId,

                    Name = incomingSet.Name,
                    Description = incomingSet.Description,
                    Weight = incomingSet.Weight
                };

                // foreach (var item in itemsFromDb) existSet.Items.Add(item);
                foreach (var item in existItemsById.Values) newSet.Items.Add(item);

                await db.Sets.AddAsync(newSet, cancel);
            }
            else
            {
                // UPDATE
                db.Entry(existSet).CurrentValues.SetValues(incomingSet);

                // Usuwanie powiązań, których już nie ma w incoming
                var incomingItems = incomingSet.Items?.ToList() ?? [];

                foreach (var existItem in existSet.Items.ToList())
                {
                    // if (!incomingItems.Any(x => x.Id == existItem.Id))
                    if (!incomingItemsIds.Contains(existItem.Id))
                        existSet.Items.Remove(existItem);
                }

                // Dodawanie nowych lub aktualizacja exist
                foreach (var incomingItem in incomingItems)
                {
                    if (existItemsById.TryGetValue(incomingItem.Id, out var existItem))
                    {
                        // Aktualizacja właściwości
                        db.Entry(existItem).CurrentValues.SetValues(incomingItem);
                        // Dodanie powiązania, jeśli go jeszcze nie ma
                        if (!existSet.Items.Any(x => x.Id == existItem.Id)) existSet.Items.Add(existItem);
                    }
                    else existSet.Items.Add(incomingItem);
                }
            }

            // SAVE
            var saved = await db.SaveChangesAsync(cancel) > 0;

            return saved ? Result.Ok().WithSuccess(LogText.SaveOk) : Result.Fail(LogText.SaveFail);
        }
        catch (OperationCanceledException e)
        {
            return Result.Fail($"{LogText.OperationCancelled} - {e.Message}");
        }
        catch (Exception e)
        {
            log.LogError(e, "{LogText}, Id: {Id}", LogText.Exception, incomingSet.Id);
            return Result.Fail([LogText.Exception, e.Message]);
        }
    }


    public async Task<Result> AddNewItemToSet(Item? incoming, Guid setId, CancellationToken cancel = default)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancel);
            var existSet = await db.Sets.Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == setId, cancellationToken: cancel);
            if (existSet is null) return Result.Fail(LogText.ObjectIsNull);

            var stub = new Item
            {
                Id = incoming.Id,
                UserId = incoming.UserId,
                Name = incoming.Name,
                Description = incoming.Description,
                Weight = incoming.Weight
            };

            existSet.Items.Add(stub);
            await db.Items.AddAsync(stub, cancel);

            var saved = await db.SaveChangesAsync(cancel) > 0;
            return saved ? Result.Ok().WithSuccess(LogText.SaveOk) : Result.Fail(LogText.SaveFail);
        }
        catch (OperationCanceledException e)
        {
            return Result.Fail($"{LogText.OperationCancelled} - {e.Message}");
        }
        catch (Exception e)
        {
            log.LogError(e, "{LogText}, Id: {Id}", LogText.Exception, incoming.Id);
            return Result.Fail([LogText.Exception, e.Message]);
        }
    }


    public async Task<Result> DeleteItemFromSet(Guid itemId, Guid setId,Guid userId,CancellationToken cancel = default)
    {
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancel);
            var existSet = await db.Sets.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == setId&&x.UserId==userId, cancel);
            if (existSet is null) return Result.Fail(LogText.ObjectIsNull);

            var existItem = existSet.Items.FirstOrDefault(x => x.Id == itemId);
            if (existItem is null) return Result.Fail(LogText.ObjectIsNull);

            existSet.Items.Remove(existItem);

            var saved = await db.SaveChangesAsync(cancel) > 0;
            return saved ? Result.Ok().WithSuccess(LogText.SaveOk) : Result.Fail(LogText.SaveFail);
        }
        catch (OperationCanceledException e)
        {
            return Result.Fail($"{LogText.OperationCancelled} - {e.Message}");
        }


        catch (Exception e)
        {
            log.LogError(e, "{LogText}, Id: {Id}", LogText.Exception, itemId);
            return Result.Fail([LogText.Exception, e.Message]);
        }
    }


/*
public async Task<Result> UpsertSet(Set? incoming, CancellationToken cancel = default)
{
    if (incoming is null) return Result.Fail(LogText.ObjectIsNull);
    await using var db = await dbFactory.CreateDbContextAsync(cancel);

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
        }#1#
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
            if (incoming.Items.Any(i => i.Id == e.Id)
                && !existSet.Items.Any(es => es.Id == e.Id))
            {
                existSet.Items.Add(e);
            }
        });

        db.Sets.Update(existSet);
    }

    var saved = await db.SaveChangesAsync(cancel) > 0;
    return saved ? Result.Ok().WithSuccess(LogText.SaveOk) : Result.Fail(LogText.SaveFail);
}
*/

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
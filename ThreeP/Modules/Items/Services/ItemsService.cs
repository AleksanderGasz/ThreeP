namespace ThreeP.Modules.Items;

public class ItemsService(IDbContextFactory<ApplicationDbContext> dbFactory, ILoggerFactory loggerFactory)
    : GenericService<Item>(dbFactory,loggerFactory)
{
        ILogger logger = loggerFactory.CreateLogger<Item>();
    public async Task<Result> UpsertItem(Item? incoming, CancellationToken cancel=default)
    {
        if (incoming is null) return Result.Fail([LogText.ObjectIsNull]);
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancel);

            var exist = await db.Items.AsNoTracking().AnyAsync(x => x.Id == incoming.Id, cancel);
            var item = new Item()
            {
                Id = incoming.Id,
                UserId = incoming.UserId
            };

            if (!exist) await db.Items.AddAsync(item, cancel);
            else db.Items.Attach(item);

            item.Name = incoming.Name;
            item.Description = incoming.Description;
            item.Weight = incoming.Weight;
            

            var saved = await db.SaveChangesAsync() > 0;
            return saved
                ? Result.Ok().WithSuccess(LogText.SaveOk)
                : Result.Fail($"{LogText.SaveFail} - {incoming.Name}");
        }
        catch (OperationCanceledException e)
        {
            return Result.Fail($"{LogText.OperationCancelled} - {e.Message}");
        }
        catch (Exception e)
        {
            logger.LogError(e, "{LogText}, Id: {Id}", LogText.Exception, incoming.Id);
            return Result.Fail([LogText.Exception, e.Message]);
        }
    }
}
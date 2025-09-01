namespace ThreeP.Modules.Items;

public class ItemsService(IDbContextFactory<ApplicationDbContext> contextFactory, ILogger<ItemsService> logger)
    : GenericService<Item>(contextFactory)
{
   public async Task<Result> UpdateItem(Item? item)
    {
        if (item is null) return Result.Fail(["Item is null"]);
        try
        {
            await using var dbContext = await contextFactory.CreateDbContextAsync();

            var exist = await dbContext.Items.FindAsync(item.Id);
            if (exist is null)
            {
                await dbContext.Items.AddAsync(item);
            }
            else
            {
                exist.Name = item.Name;
                exist.Description = item.Description;
                exist.Weight = item.Weight;
            }

            await dbContext.SaveChangesAsync();
            return Result.Ok().WithSuccess("Zapisano pomyślnie");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error updating item {ItemId}", item.Id);
            return Result.Fail(["Error updating item", e.Message]);
        }
    }
}
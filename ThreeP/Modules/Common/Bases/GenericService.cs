namespace Mac.Modules.Bases;

public class GenericService<T>(IDbContextFactory<ApplicationDbContext> dbFactory, ILoggerFactory loggerFactory)
    where T : BaseModel, new()
{
    readonly ILogger<T> log = loggerFactory.CreateLogger<T>();


    //GET
    public async Task<List<T?>> Get(
        List<Expression<Func<T, object>>>? includes = null,
        List<Expression<Func<T, bool>>>? filters = null,
        Expression<Func<T, object>>? orderBy = null,
        bool asNoTracking = true)
    {
        await using var dbContext = await dbFactory.CreateDbContextAsync();

        IQueryable<T> query = dbContext.Set<T>();
        if (includes is not null && includes.Any()) includes.ForEach(x => { query = query.Include(x); });
        if (filters is not null && filters.Any()) filters.ForEach(x => { query = query.Where(x); });
        if (orderBy is not null) query = query.OrderBy(orderBy);

        if (asNoTracking) return await query.AsNoTracking().ToListAsync();
        return await query.ToListAsync();
    }


    public async Task<T?> GetOne(
        List<Expression<Func<T, object>>>? includes = null,
        List<Expression<Func<T?, bool>>>? filters = null,
        Expression<Func<T, object>>? orderBy = null,
        bool asNoTracking = true)
    {
        await using var dbContext = await dbFactory.CreateDbContextAsync();

        IQueryable<T?> query = dbContext.Set<T>();
        if (includes is not null && includes.Any()) includes.ForEach(x => { query = query.Include(x); });
        if (filters is not null && filters.Any()) filters.ForEach(x => { query = query.Where(x); });
        if (orderBy is not null)
        {
            query = query.OrderByDescending(orderBy);
        }


        if (asNoTracking) return await query.AsNoTracking().FirstOrDefaultAsync();
        return await query.FirstOrDefaultAsync();
    }


    public async Task<T> GetById(
        Guid? id,
        List<Expression<Func<T, object>>>? includes = null,
        List<Expression<Func<T, bool>>>? filters = null,
        bool asNoTracking = true)
    {
        if (id is null) return null;
        await using var dbContext = await dbFactory.CreateDbContextAsync();

        IQueryable<T> query = dbContext.Set<T>();
        if (includes is not null && includes.Any()) includes.ForEach(x => { query = query.Include(x); });
        if (filters is not null && filters.Any()) filters.ForEach(x => { query = query.Where(x); });
        if (asNoTracking)return await query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return await query.FirstOrDefaultAsync(x=>x.Id==id);
    }


    //? UPSERT
    public async Task<Result> Upsert(T? incoming, Action<T, T> map, CancellationToken cancel)
    {
        if (incoming is null) return Result.Fail(LogText.ObjectIsNull);
        try
        {
            await using var db = await dbFactory.CreateDbContextAsync(cancel);
            var exist = await db.Set<T>().AsNoTracking().AnyAsync(x => x.Id == incoming.Id, cancel);
            var item = new T { Id = incoming.Id};

            if (!exist) await db.Set<T>().AddAsync(item, cancel);
            else db.Set<T>().Attach(item);

            map(incoming, item);

            var saved = await db.SaveChangesAsync() > 0;
            return saved
                ? Result.Ok().WithSuccess($"{LogText.SaveOk} - Id: {incoming.Id}")
                : Result.Fail($"{LogText.SaveFail} - Id: {incoming.Id} ");
        }
        catch (OperationCanceledException e)
        {
            log.LogWarning(e, "{LogText}, Id: {Id}", LogText.Exception, incoming.Id);
            return Result.Fail($"{LogText.OperationCancelled} - (Id: {incoming.Id}) {e.Message}");
        }
        catch (Exception e)
        {
            log.LogError(e, "{LogText}, Id: {Id}", LogText.Exception, incoming.Id);
            return Result.Fail($"{LogText.Exception} - (Id: {incoming.Id}) {e.Message}");
        }
    }


//*ADD
    public async Task Add(T? item)
    {
        if (item is null) return;
        await using var dbContext = await dbFactory.CreateDbContextAsync();
        await dbContext.Set<T>().AddAsync(item);
        await dbContext.SaveChangesAsync();
    }


    //?UPDATE
    public async Task Update(T? item)
    {
        if (item is null) return;
        await using var dbContext = await dbFactory.CreateDbContextAsync();


        var exist = await dbContext.Set<T>()
            .FirstOrDefaultAsync(x => x.Id == item.Id);

        if (exist is null) return;
        dbContext.Entry(exist).CurrentValues.SetValues(item);
        dbContext.Entry(exist).State = EntityState.Modified;


        await dbContext.SaveChangesAsync();
    }


//!DELETE
    /*public async Task Delete(Guid? id)
    {
        if (id is null) return;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var exist = await dbContext.Set<T>().FindAsync(id);
        if (exist is null) return;

        dbContext.Set<T>().Remove(exist);
        await dbContext.SaveChangesAsync();
    }*/
    public async Task<Result> Delete(Guid? id)
    {
        if (id is null) return Result.Fail(LogText.ObjectIsNull);
        await using var dbContext = await dbFactory.CreateDbContextAsync();
        var exist = await dbContext.Set<T>().FindAsync(id);
        if (exist is null) return Result.Fail(LogText.ObjectIsNull);


        dbContext.Set<T>().Remove(exist);
        var saveResult = await dbContext.SaveChangesAsync();
        if (saveResult <= 0) return Result.Fail(LogText.DeleteFail);
        return Result.Ok().WithSuccess(LogText.DeletedOk);
    }
}
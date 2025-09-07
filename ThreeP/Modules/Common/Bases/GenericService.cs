namespace Mac.Modules.Bases;

public class GenericService<T>(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    where T : BaseModel, new()
{
    //GET
    public async Task<List<T>> Get(
        List<Expression<Func<T, object>>>? includes = null,
        List<Expression<Func<T, bool>>>? filters = null,
        Expression<Func<T, object>>? orderBy = null,
        bool asNoTracking = true)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        IQueryable<T> query = dbContext.Set<T>();
        if (includes is not null && includes.Any()) includes.ForEach(x => { query = query.Include(x); });
        if (filters is not null && filters.Any()) filters.ForEach(x => { query = query.Where(x); });
        if (orderBy is not null) query = query.OrderBy(orderBy);

        if (asNoTracking) return await query.AsNoTracking().ToListAsync();
        return   await query.ToListAsync();
    }


    public async Task<T?> GetOne(
        List<Expression<Func<T, object>>>? includes = null,
        List<Expression<Func<T?, bool>>>? filters = null,
        Expression<Func<T, object>>? orderBy = null,
        bool asNoTracking = true)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

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
        List<Expression<Func<T, bool>>>? filters = null)
    {
        if (id is null) return null;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        IQueryable<T> query = dbContext.Set<T>();
        if (includes is not null && includes.Any()) includes.ForEach(x => { query = query.Include(x); });
        if (filters is not null && filters.Any()) filters.ForEach(x => { query = query.Where(x); });
        return await query.FirstOrDefaultAsync();
    }


//*ADD
    public async Task Add(T? item)
    {
        if (item is null) return;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Set<T>().AddAsync(item);
        await dbContext.SaveChangesAsync();
    }


    //?UPDATE
    public async Task Update(T? item)
    {
        if (item is null) return;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();


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
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var exist = await dbContext.Set<T>().FindAsync(id);
        if (exist is null) return Result.Fail(LogText.ObjectIsNull);


        dbContext.Set<T>().Remove(exist);
        var saveResult = await dbContext.SaveChangesAsync();
        if (saveResult <= 0) return Result.Fail(LogText.CantDelete);
            return Result.Ok().WithSuccess(LogText.ObjectDeleted);
    }
}
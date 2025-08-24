namespace Mac.Modules.Bases;

public class GenericServiceWithUser<T>(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    where T : BaseModelWithUser, new()
{
    //GET
    public async Task<List<T>> Get(string? userName
        , List<Expression<Func<T, object>>>? includes = null,
        List<Expression<Func<T, bool>>> filters = null,
        Expression<Func<T, object>> orderBy = null,
        bool AsNoTracking = true)
    {
        if (userName is null) return [];
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        IQueryable<T> query = dbContext.Set<T>();
        query = query.Where(x => x.User != null && x.User.UserName == userName);
        if (includes is not null && includes.Any()) includes.ForEach(x => { query = query.Include(x); });
        if (filters is not null && filters.Any()) filters.ForEach(x => { query = query.Where(x); });
        if (orderBy is not null)
        {
            query = query.OrderBy(orderBy);
        }

        ;

        if (AsNoTracking) return await query.AsNoTracking().ToListAsync();
        return await query.ToListAsync();
    }


    public async Task<T> GetOne(string? userName
        , List<Expression<Func<T, object>>>? includes = null,
        List<Expression<Func<T, bool>>> filters = null,
        Expression<Func<T, object>> orderBy = null,
        bool AsNoTracking = true)
    {
        if (userName is null) return new T();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        IQueryable<T> query = dbContext.Set<T>();
        query = query.Where(x => x.User != null && x.User.UserName == userName);
        if (includes is not null && includes.Any()) includes.ForEach(x => { query = query.Include(x); });
        if (filters is not null && filters.Any()) filters.ForEach(x => { query = query.Where(x); });
        if (orderBy is not null)
        {
            query = query.OrderByDescending(orderBy);
        }

        ;

        if (AsNoTracking) return await query.AsNoTracking().FirstOrDefaultAsync();
        return await query.FirstOrDefaultAsync();
    }


    public async Task<T> GetById(Guid? id, string? userName, List<Expression<Func<T, object>>>? includes = null,
        List<Expression<Func<T, bool>>>? filters = null)
    {
        if (id is null) return null;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        IQueryable<T> query = dbContext.Set<T>();
        query = query.Where(x => x.User != null && x.User.UserName == userName && x.Id == id);
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
            .Where(x => x.User.Id == item.UserId)
            .FirstOrDefaultAsync(x => x.Id == item.Id);

        if (exist is null) return;
        dbContext.Entry(exist).CurrentValues.SetValues(item);
        dbContext.Entry(exist).State = EntityState.Modified;


        await dbContext.SaveChangesAsync();
    }


//!DELETE
    public async Task Delete(Guid? id)
    {
        if (id is null) return;
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var exist = await dbContext.Set<T>().FindAsync(id);
        if (exist is null) return;

        dbContext.Set<T>().Remove(exist);
        await dbContext.SaveChangesAsync();
    }
}
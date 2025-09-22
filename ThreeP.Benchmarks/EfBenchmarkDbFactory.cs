namespace ThreeP.Benchmarks;

public class EfBenchmarkDbFactory : IDbContextFactory<ApplicationDbContext>, IAsyncDisposable
{
    readonly SqliteConnection connection;
    readonly DbContextOptions<ApplicationDbContext> options;
    public Guid SeededUserId { get; private set; }


    public EfBenchmarkDbFactory()
    {
        connection = new SqliteConnection("Data Source=:memory:;Cache=Shared;Foreign Keys=False");
        connection.Open();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);

        options = optionsBuilder.Options;

        using var db = new ApplicationDbContext(options);
        db.Database.EnsureCreated();
    }


    public ApplicationDbContext CreateDbContext() => new(options);


    public ValueTask<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(new ApplicationDbContext(options));


    public async Task Seed(int count, Guid? userId = null, CancellationToken cancel = default)
    {
        await using var db = CreateDbContext();
        if (await db.Items.AnyAsync(cancel)) return;

        var uid = userId??Guid.CreateVersion7();
        SeededUserId = uid;
        
        var rnd = new Random(42);

        var items = Enumerable.Range(1, count).Select(x => new Item
        {
            Id = Guid.CreateVersion7(),
            UserId = uid, // FK nieegzekwowany (Foreign Keys=False)
            Name = $"Item {x}",
            Description = x % 5 == 0 ? $"Opis {x}" : null,
            Weight = (float)Math.Round(rnd.NextDouble() * 2.5, 2)
        });

        await db.Items.AddRangeAsync(items, cancel);
        await db.SaveChangesAsync(cancel);
    }


    public async ValueTask DisposeAsync()
    {
        await connection.CloseAsync();
        connection.Dispose();
    }


    public static ILoggerFactory CreateNullLoggerFactory() => NullLoggerFactory.Instance;
}
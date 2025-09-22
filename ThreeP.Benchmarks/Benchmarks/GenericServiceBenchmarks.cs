namespace ThreeP.Benchmarks;

[MemoryDiagnoser]
public class GenericServiceBenchmarks
{
     EfBenchmarkDbFactory factory = null!;
     GenericService<Item> service = null!;
     Guid userId;

     [Params(1_000,10_000)]
     public int N;


     [GlobalSetup]
     public async Task Setup()
     {
         factory = new EfBenchmarkDbFactory();
         userId=Guid.CreateVersion7();
         await factory.Seed(N, userId);

         service = new GenericService<Item>(factory, EfBenchmarkDbFactory.CreateNullLoggerFactory());

         _ = await service.Get(filters: [x => x.UserId == userId], asNoTracking: true);
         
     }


     [Benchmark(Baseline = true)]
     public async Task<List<Item>> Get_Filtered_AsNoTracking() => await service.Get(filters: [x => x.UserId == userId], asNoTracking: true);
     
     [Benchmark]
     public async Task<List<Item>> Get_Filtered_Tracked() => await service.Get(filters: [x => x.UserId == userId], asNoTracking: false);

     
     [Benchmark]
     public async Task<List<Item>> Get_Filtered_OrderedByName() =>
         await service.Get(filters: [x => x.UserId == userId], orderBy: x => x.Name, asNoTracking: true);

     
     

     [GlobalCleanup]
     public async Task Cleanup()
     {
         if (factory is IAsyncDisposable d) await d.DisposeAsync();
     }
}
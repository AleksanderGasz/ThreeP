namespace ThreeP.Benchmarks;

[MemoryDiagnoser] // mierzymy alokacje
public class RoundNumbersBenchmarks
{
    float?[] _data = [];

    [Params(1_000, 100_000)] public int N; // <- rozmiar wejścia (parametryzowane przez BDN)


    [GlobalSetup]
    public void Setup()
    {
        // Przygotuj dane testowe raz na cały cykl benchmarku
        var rnd = new Random(42);
        _data = Enumerable.Range(0, N)
            .Select(_ => (float?)((float)rnd.NextDouble() * 1000f))
            .ToArray();
    }


    [Benchmark(Baseline = true)]
    public float MathF_Round()
    {
        // Referencyjna implementacja na MathF.Round
        float sum = 0f;
        foreach (var v in _data)
            sum += MathF.Round(v ?? 0f, 2);
        return sum;
    }


    [Benchmark]
    public float RoundNumbers_Extension()
    {
        // Testowana metoda: RoundNumbers.Round(...)
        float sum = 0f;
        foreach (var v in _data)
            sum += v.Round(2); // <- rozszerzenie z projektu ThreeP
        return sum;
    }
}
namespace BlinKayTest.Shared
{
    public interface IBenchmarkRepository
    {
        Task AddAsync(Benchmark benchmarkToAdd);

        Task<Benchmark?> FindByIdAsync(int benchmarkId);

        Task<List<int>> GetAllIdsAsync();

    }
}

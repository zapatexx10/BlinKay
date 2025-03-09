using BlinKayTest.Infrastructure.SQL.Context;
using BlinKayTest.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlinKayTest.Infrastructure.SQL
{
    public class BenchmarkRepository : IBenchmarkRepository
    {
        private readonly AppDbContext _appDbContext;

        public BenchmarkRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task AddAsync(Benchmark benchmarkToAdd)
        {
            await _appDbContext.Benchmarks.AddAsync(benchmarkToAdd);
        }

        public async Task<Benchmark?> FindByIdAsync(int benchmarkId)
        {
            return await _appDbContext.Benchmarks.FirstOrDefaultAsync(
                b=> b.Id == benchmarkId);
        }

        public async Task<List<int>> GetAllIdsAsync()
        {
            return await _appDbContext.Benchmarks.Select(b => b.Id).ToListAsync();
        }
    }
}

using BlinKayTest.Contracts;
using BlinKayTest.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace BlinkayTest.Application
{
    public class BenchmarkService : ServiceBase<BenchmarkService>, IBenchmarkService
    {
        //Start using service provider in order to avoid problems with DbContext (not thread safe) adn thread concurrency.
        private readonly IServiceProvider _serviceProvicer;

        public BenchmarkService(IUnitOfWork unitOfWork,
                                IServiceProvider serviceProvicer)
            : base(unitOfWork)
        {
            _serviceProvicer = serviceProvicer;
        }

        #region .: Public Methods :.

        public async Task<long> PGInsertion(int iNumRegistries, int iNumThreads)
        {
            return await ExecuteBenchmark(iNumRegistries, iNumThreads, async (serviceProvider) =>
            {
                var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
                var repository = serviceProvider.GetRequiredService<IBenchmarkRepository>();

                await unitOfWork.BeginTransactionAsync();

                try
                {
                    await AddNewBenchmark(unitOfWork, repository);
                    await unitOfWork.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
        }

        public async Task<long> PGSelectPlusUpdate(int iNumRegistries, int iNumThreads)
        {
            return await ExecuteBenchmark(iNumRegistries, iNumThreads, async (serviceProvider) =>
            {
                var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
                var repository = serviceProvider.GetRequiredService<IBenchmarkRepository>();

                await unitOfWork.BeginTransactionAsync();
                try
                {
                    Benchmark? benchmarkToUpdate = await GetRandomBenchmark(repository);

                    if (benchmarkToUpdate is not null)
                    {
                        benchmarkToUpdate.UpdateName(Guid.NewGuid().ToString());
                        await unitOfWork.SaveChangesAsync();
                    }
                    await unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
        }

        public async Task<long> PGSelectPlusUpdatePlusInsertion(int iNumRegistries, int iNumThreads)
        {
            return await ExecuteBenchmark(iNumRegistries, iNumThreads, async (serviceProvider) =>
            {
                var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
                var repository = serviceProvider.GetRequiredService<IBenchmarkRepository>();

                await unitOfWork.BeginTransactionAsync();
                try
                {
                    var randomId = new Random().Next(1, iNumRegistries + 1);
                    var entity = await repository.FindByIdAsync(randomId);

                    if (entity is not null)
                    {
                        entity.UpdateName(Guid.NewGuid().ToString());
                        await unitOfWork.SaveChangesAsync();
                    }

                    await AddNewBenchmark(unitOfWork, repository);

                    await unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });
        }

        #endregion

        #region .: Private Methods :.

        private async Task AddNewBenchmark(IUnitOfWork unitOfWork, IBenchmarkRepository repository)
        {
            var randomName = Guid.NewGuid().ToString();
            await repository.AddAsync(new Benchmark(randomName));
            await unitOfWork.SaveChangesAsync();
        }

        private async Task<Benchmark?> GetRandomBenchmark(IBenchmarkRepository repository)
        {
            var allBenchmarkIds = await repository.GetAllIdsAsync();

            if (!allBenchmarkIds.Any())
            {
                throw new Exception("There is no available id in the database");
            }

            var randomId = allBenchmarkIds[new Random().Next(0, allBenchmarkIds.Count)];
            var benchmarkToUpdate = await repository.FindByIdAsync(randomId);
            return benchmarkToUpdate;
        }

        private async Task<long> ExecuteBenchmark(int iNumRegistries,
                                                  int iNumThreads,
                                                  Func<IServiceProvider, Task> operation)
        {
            var stopwatch = Stopwatch.StartNew();
            var tasks = new List<Task>();

            for (int i = 0; i < iNumThreads; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    using (var scope = _serviceProvicer.CreateScope())
                    {
                        for (int j = 0; j < iNumRegistries; j++)
                        {
                            await operation(scope.ServiceProvider);
                        }
                    }
                }));
            }

            //We wait all the thread executions
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private async Task<long> RunBenchmarkParallel(int iNumRegistries, int iNumThreads, Func<Task> action)
        {
            var stopwatch = Stopwatch.StartNew();

            Parallel.ForEach(
                Enumerable.Range(0, iNumThreads),
                new ParallelOptions { MaxDegreeOfParallelism = iNumThreads },
                (threadIndex) =>
                {
                    Task.Run(async () =>
                    {
                        for (int j = 0; j < iNumRegistries; j++)
                        {
                            await action();
                        }
                    }).Wait();
                });

            stopwatch.Stop();
            await Task.CompletedTask;
            return stopwatch.ElapsedMilliseconds;
        }

        #endregion
    }
}

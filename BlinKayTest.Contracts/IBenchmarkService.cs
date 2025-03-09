namespace BlinKayTest.Contracts
{
    public interface IBenchmarkService
    {
        Task<long> PGInsertion(int iNumRegistries, int iNumThreads);
        Task<long> PGSelectPlusUpdate(int iNumRegistries, int iNumThreads);
        Task<long> PGSelectPlusUpdatePlusInsertion(int iNumRegistries, int iNumThreads);
    }
}

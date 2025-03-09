
namespace BlinKayTest.Shared
{
    public class Benchmark
    {
        public int Id { get; private set; }

        public string Name { get; private set; } = null!;

        public Benchmark(string name)
        {
            Name = name;
        }

        public Benchmark(int id, string name)
        {
            Name = name;
        }

        public void UpdateName(string newName)
        {
            Name = newName;
        }
    }
}

using BlinKayTest.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlinKayTest.Infrastructure.SQL.Context;

public class AppDbContext : DbContext
{
    public DbSet<Benchmark> Benchmarks => Set<Benchmark>();

    public AppDbContext(DbContextOptions options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Benchmark>(entity =>
        {
            entity.HasKey(e=> e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired();
        });
    }
}

using BlinKayTest.Infrastructure.SQL.Context;
using Microsoft.EntityFrameworkCore;

namespace BlinKayTest.API.Helpers;

public static class MigrationsHelper
{
    public static async Task MigrateDatabaseAsync(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying migrations: {ex.Message}");
                throw;
            }
        }
    }
}
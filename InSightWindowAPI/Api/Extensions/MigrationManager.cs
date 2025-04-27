using Domain.Entity.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Domain.EntityFramework;

public static class MigrationManager
{
    /// <summary>
    ///     Migrates database on a given Database context type.
    /// </summary>
    /// <param name="app"></param>
    public static IHost MigrateDatabase<T>(this IHost host) where T : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        using var appContext = services.GetRequiredService<T>();
        try
        {
            appContext.Database.Migrate();
            if (appContext is UsersContext)
            {
                RoleSeeder.SeedRolesAsync(services).Wait();
            }
        }
        catch (Exception ex)
        {
            var writer = Console.Error;
            writer.WriteLine(ex);
        }

        return host;
    }

}

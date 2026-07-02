using GYMsystem.DAL.Context;
using GYMsystem.DAL.DataSeeding;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;

namespace MVC1
{
    public static class ProgramExtension
    {
        public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
        {

            using var scope = app.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<GYMDBContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var pendingMigration = await dbcontext.Database.GetPendingMigrationsAsync();
            if (pendingMigration != null)
            {
                logger.LogInformation($"Appling{pendingMigration.Count()} pending migration");
                await dbcontext.Database.MigrateAsync();
            }
            var FolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GYMDataSeeding.SeedData(dbcontext, FolderPath, logger);
        }

    }
}

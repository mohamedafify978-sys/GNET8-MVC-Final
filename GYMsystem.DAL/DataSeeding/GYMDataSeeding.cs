using GYMsystem.DAL.Context;
using GYMsystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GYMsystem.DAL.DataSeeding
{
    public class GYMDataSeeding
    {
        public static async Task SeedData(GYMDBContext dbcontext, string seedFolderpath, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                if (!await dbcontext.Plans.AnyAsync(ct))
                {
                    var plans = LoadDataFromJsonFile<Plan>(seedFolderpath ,"plans.json");
                    if(plans.Any())
                        dbcontext.Plans.AddRange(plans);
                    
                    logger.LogInformation($"plans seeded with count = {plans.Count}");


                }
                if (dbcontext.ChangeTracker.HasChanges()) 
                     await  dbcontext.SaveChangesAsync(ct);                   
                 else 
                    logger.LogInformation($"plan already seeded");

            }
            catch (Exception ex) 
            {
                logger.LogError(ex, "GYM data seeding Failed");
                throw;

            }

        }
        private static List<T> LoadDataFromJsonFile<T>(string folderPath, string fileName)
        {

            var filePath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Seed data file not found: {filePath}");

            var data = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new JsonStringEnumConverter());

            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
        }
}

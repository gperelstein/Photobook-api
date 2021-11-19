using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Photobook.Data;
using System;
using System.IO;

namespace Photobook.MigrationUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Applying migrations");
            var webHost = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<MigrationUtilityStartup>()
                .Build();
            using (var context = (PhotobookDbContext)webHost.Services.GetService(typeof(PhotobookDbContext)))
            {
                context.Database.Migrate();
            }
            Console.WriteLine("Done");
        }
    }
}

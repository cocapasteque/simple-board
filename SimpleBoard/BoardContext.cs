using System;
using System.Linq;
using Lisb.Utils.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SimpleBoard
{
    public class BoardContext : DbContext
    {
        public virtual DbSet<BoardEntry> Entries { get; set; }

        public BoardContext(DbContextOptions<BoardContext> options)
            : base(options)
        {
        }

        protected BoardContext(DbContextOptions options) : base(options)
        {
        }
        
        public static void UpdateDatabase<T>(IApplicationBuilder app) where T: DbContext
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<T>();

            Console.WriteLine("Checking for database updates.");
            if (context != null && context.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("Migrating database to latest migration.");
                context.Database.Migrate();
            }
            else
            {
                Console.WriteLine("No migration available.");
            }
        }
        
        public static string GetDbConnectionString(string db)
        {
            var svc = Service.GetService("infra", "mysql8", "mysql");
#if DEBUG
            return $"Server=localhost;Database={db};Uid=root;Pwd=nEwxai31h7IgYHN7PgRd;";
#else
            return $"Server={svc.Host};Database={db};Uid=root;Pwd=nEwxai31h7IgYHN7PgRd";
#endif
        }
    }
}
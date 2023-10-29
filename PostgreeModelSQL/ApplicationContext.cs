using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PostgreeModelSQL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreeModelSQL
{
    public class ApplicationContext : DbContext
    {
        public virtual DbSet<WeatherArchive> WeatherArchive { get; set; }
        public virtual DbSet<WeatherArchiveDaily> WeatherArchiveDaily { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile(@"C:\\Users\\Vadim\\Desktop\\C#\\MoscowWeather\\PostgreeModelSQL\\settings.json");
            var connectionString = builder.Build().GetConnectionString("ConnectionString");
            optionsBuilder.UseLazyLoadingProxies().UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherArchiveDaily>(c =>
            {
                c.HasNoKey();
                c.ToView("DailyArchiveByDay");
            });
        }
    }
}

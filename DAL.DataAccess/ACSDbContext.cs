using System.IO;
using DAL.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.DataAccess
{
    public class ACSDbContext : DbContext
    {
        public ACSDbContext()
        {
        }

        public ACSDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            string currentDirectory = Directory.GetCurrentDirectory();
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(currentDirectory).AddJsonFile("appsettings.json").Build();

            string connectionString = configuration.GetConnectionString("ACSConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ShortenerURLConfiguration());

        }
    }
}
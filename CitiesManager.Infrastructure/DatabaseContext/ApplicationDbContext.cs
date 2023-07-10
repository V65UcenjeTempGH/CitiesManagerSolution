using CitiesManager.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CitiesManager.WebAPI.DatabaseContext
{
    /// <summary>
    /// Jednostavan primer
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) :base(options)
        {
        }
        public ApplicationDbContext()
        {
        }

        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<City>().HasData(
                new City
                {
                    CityID = Guid.Parse("643B309F-D8C1-4B85-B48A-285CFDD69114"), 
                    CityName = "Зрењанин",
                    DateOfFoundation= new DateTime(1500,1,1),
                    Population = 90000,
                    ZipCode = "23000",
                    CityHistory="...",
                    Description="..."
                }
            );

            modelBuilder.Entity<City>().HasData(
                new City 
                { 
                    CityID = Guid.Parse("459A76F0-F918-4965-B197-D8E0A3EDEB27"), 
                    CityName = "Београд",
                    DateOfFoundation = new DateTime(1500, 1, 1),
                    Population = 2200000,
                    ZipCode = "11000",
                    CityHistory = "...",
                    Description = "..."
                });

            modelBuilder.Entity<City>().HasData(
                new City 
                { 
                    CityID = Guid.Parse("B6F78464-4403-4210-949F-236D28753FDD"), 
                    CityName = "Нови Сад", 
                    DateOfFoundation = new DateTime(1500, 1, 1),
                    Population = 450000,
                    ZipCode = "21000",
                    CityHistory = "...",
                    Description = "..."
                });

        }
    }
}

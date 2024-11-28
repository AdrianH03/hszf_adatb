using ABC123_HSZF_2024251.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ABC123_HSZF_2024251.Persistence.MsSql;

namespace ABC123_HSZF_2024251.Test.DataImporterTests
{
    public class DataImporterTests
    {
        private TaxiDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TaxiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TaxiDbContext(options);
        }
        [Fact]
        public async Task ImportDataAsync_ShouldImportTaxiCarsFromJsonFile()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            // Töröljük a meglévő adatokat, hogy tiszta állapotról induljunk
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            var service = new DataImporterService(dbContext);

            // Ideiglenes JSON fájl létrehozása
            var jsonData = @"
            {
                ""TaxiCars"": [
                {
                    ""LicensePlate"": ""ABC123"",
                    ""Driver"": ""John Doe"",
                    ""Fares"": []
                }
                ]
            }";

            var tempFilePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempFilePath, jsonData);

            try
            {
                // Act
                await service.ImportDataAsync(tempFilePath);

                // Assert
                var cars = await dbContext.TaxiCars.ToListAsync();
                Assert.Single(cars); // Ellenőrizzük, hogy pontosan 1 rekord van
                Assert.Equal("ABC123", cars[0].LicensePlate);
                Assert.Equal("John Doe", cars[0].Driver);
            }
            finally
            {
                // Takarítás: ideiglenes fájl törlése
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
            }
        }
    }
}

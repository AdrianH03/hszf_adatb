using ABC123_HSZF_2024251.Application.Services;
using ABC123_HSZF_2024251.Model;
using ABC123_HSZF_2024251.Persistence.MsSql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ABC123_HSZF_2024251.Test.ServiceTests
{
    public class StatisticsServiceTests
    {
        private TaxiDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TaxiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TaxiDbContext(options);
        }
        [Fact]
        public async Task GetAverageDistanceAsync_ShouldReturnCorrectAverage()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();
            var service = new StatisticsService(dbContext);
            var car = new TaxiCar
            {
                LicensePlate = "DEF456",
                Driver = "Jane Smith",
                Fares = new List<Fare>
                {
                    new Fare { Distance = 10 },
                    new Fare { Distance = 20 },
                    new Fare { Distance = 30 }
                }
            };

            dbContext.TaxiCars.Add(car);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(20.0, result["DEF456"]);
        }

    }
}

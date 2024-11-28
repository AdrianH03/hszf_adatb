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
        new Fare { Distance = 10, From = "A", To = "B" },
        new Fare { Distance = 20, From = "C", To = "D" },
        new Fare { Distance = 30, From = "E", To = "F" }
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

        [Fact]
        public async Task GetAverageDistanceAsync_ShouldReturnEmpty_WhenNoTaxiCarsExist()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();
            var service = new StatisticsService(dbContext);

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAverageDistanceAsync_ShouldReturnZero_WhenTaxiCarHasNoFares()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            dbContext.TaxiCars.Add(new TaxiCar
            {
                LicensePlate = "XYZ789",
                Driver = "Jane Doe",
                Fares = new List<Fare>() // Nincs viteldíj
            });
            await dbContext.SaveChangesAsync();

            var service = new StatisticsService(dbContext);

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(0, result["XYZ789"]);
        }
        [Fact]
        public async Task GetAverageDistanceAsync_ShouldCalculateAverageForSingleTaxiCar()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            dbContext.TaxiCars.Add(new TaxiCar
            {
                LicensePlate = "DEF456",
                Driver = "Alice Smith",
                Fares = new List<Fare>
        {
            new Fare { Distance = 15.0, From = "A", To = "B"},
            new Fare { Distance = 5.0, From = "C", To = "D"},
            new Fare { Distance = 10.0, From = "E", To = "F"  }
        }
            });
            await dbContext.SaveChangesAsync();

            var service = new StatisticsService(dbContext);

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(10.0, result["DEF456"]);
        }

        [Fact]
        public async Task GetAverageDistanceAsync_ShouldCalculateForMultipleTaxiCars()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            dbContext.TaxiCars.AddRange(
                new TaxiCar
                {
                    LicensePlate = "ABC123",
                    Driver = "John Doe",
                    Fares = new List<Fare>
                    {
                new Fare { Distance = 10.0, From = "E", To = "F" },
                new Fare { Distance = 20.0, From = "A", To = "B" }
                    }
                },
                new TaxiCar
                {
                    LicensePlate = "DEF456",
                    Driver = "Jane Smith",
                    Fares = new List<Fare>
                    {
                new Fare { Distance = 5.0, From = "C", To = "D" },
                new Fare { Distance = 15.0, From = "A", To = "B"  }
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var service = new StatisticsService(dbContext);

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(15.0, result["ABC123"]);
            Assert.Equal(10.0, result["DEF456"]);
        }

        [Fact]
        public async Task GetAverageDistanceAsync_ShouldHandleTaxiCarsWithAndWithoutFares()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            dbContext.TaxiCars.AddRange(
                new TaxiCar
                {
                    LicensePlate = "ABC123",
                    Driver = "John Doe",
                    Fares = new List<Fare>
                    {
                new Fare {Distance = 15.0, From = "A", To = "B"}
                    }
                },
                new TaxiCar
                {
                    LicensePlate = "DEF456",
                    Driver = "Jane Smith",
                    Fares = new List<Fare>()
                }
            );
            await dbContext.SaveChangesAsync();

            var service = new StatisticsService(dbContext);

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(15.0, result["ABC123"]);
            Assert.Equal(0.0, result["DEF456"]);
        }
        [Fact]
        public async Task GetAverageDistanceAsync_ShouldIgnoreNegativeDistances()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            dbContext.TaxiCars.Add(new TaxiCar
            {
                LicensePlate = "XYZ123",
                Driver = "Bob Brown",
                Fares = new List<Fare>
        {
            new Fare { Distance = -5.0, From = "A", To = "B" },
            new Fare { Distance = 15.0, From = "C", To = "D" },
            new Fare {Distance = 10.0, From = "A", To = "B"}
        }
            });
            await dbContext.SaveChangesAsync();

            var service = new StatisticsService(dbContext);

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(12.5, result["XYZ123"]);
        }
        [Fact]
        public async Task GetAverageDistanceAsync_ShouldIgnoreTaxiCarsWithEmptyLicensePlate()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            dbContext.TaxiCars.Add(new TaxiCar
            {
                LicensePlate = "",
                Driver = "Unnamed",
                Fares = new List<Fare>
        {
            new Fare { Distance = 10.0, From = "A", To = "B"}
        }
            });
            await dbContext.SaveChangesAsync();

            var service = new StatisticsService(dbContext);

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAverageDistanceAsync_ShouldNotDuplicateKeys()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            dbContext.TaxiCars.AddRange(
                new TaxiCar
                {
                    LicensePlate = "ABC123",
                    Driver = "John Doe",
                    Fares = new List<Fare>
                    {
                new Fare {Distance = 10.0, From = "A", To = "B"}
                    }
                },
                new TaxiCar
                {
                    LicensePlate = "ABC123",
                    Driver = "Jane Smith",
                    Fares = new List<Fare>
                    {
                new Fare { Distance = 15.0 , From = "C", To = "D" }
                    }
                }
            );
            await dbContext.SaveChangesAsync();

            var service = new StatisticsService(dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await service.GetAverageDistanceAsync();
            });
        }
        [Fact]
        public async Task GetAverageDistanceAsync_ShouldHandleLargeDataSet()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            dbContext.TaxiCars.RemoveRange(dbContext.TaxiCars);
            dbContext.Fares.RemoveRange(dbContext.Fares);
            await dbContext.SaveChangesAsync();

            for (int i = 1; i <= 1000; i++)
            {
                dbContext.TaxiCars.Add(new TaxiCar
                {
                    LicensePlate = $"CAR{i:D4}",
                    Driver = $"Driver {i}",
                    Fares = new List<Fare>
            {
                new Fare {Distance = i * 1.0, From = "A", To = "B"}
            }
                });
            }
            await dbContext.SaveChangesAsync();

            var service = new StatisticsService(dbContext);

            // Act
            var result = await service.GetAverageDistanceAsync();

            // Assert
            Assert.Equal(1000, result.Count);
            Assert.Equal(500.5, result.Values.Average());
        }

    }
}

using System.Threading.Tasks;
using Xunit;
using ABC123_HSZF_2024251.Persistence.MsSql;
using ABC123_HSZF_2024251.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using ABC123_HSZF_2024251.Application.Services;

public class CarManagementServiceTests
{
    private TaxiDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<TaxiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TaxiDbContext(options);
    }

    [Fact]
    public async Task AddFareAsync_ShouldAddFareToCar()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var service = new CarManagementService(dbContext);
        var car = new TaxiCar { LicensePlate = "ABC123", Driver = "John Doe", Fares = new List<Fare>() };
        dbContext.TaxiCars.Add(car);
        await dbContext.SaveChangesAsync();

        var newFare = new Fare
        {
            From = "Point A",
            To = "Point B",
            Distance = 10.5,
            PaidAmount = 2500,
            FareStartDate = DateTime.Now
        };

        string notificationMessage = null;
        Action<string> notification = message =>
        {
            notificationMessage = message;
        };

        // Act
        await service.AddFareAsync("ABC123", newFare, notification);

        // Assert
        var updatedCar = await dbContext.TaxiCars.Include(c => c.Fares).FirstOrDefaultAsync(c => c.LicensePlate == "ABC123");
        Assert.Single(updatedCar.Fares);
        Assert.Equal("Point A", updatedCar.Fares.First().From);
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
                new Fare { Distance = 10, From = "C", To = "D" },
                new Fare { Distance = 20, From = "A", To = "B"  },
                new Fare { Distance = 30, From = "E", To = "F"  }
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
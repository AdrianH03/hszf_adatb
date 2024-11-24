using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ABC123_HSZF_2024251.Model;
using ABC123_HSZF_2024251.Persistence.MsSql;
using ABC123_HSZF_2024251.Application.Interfaces;
namespace ABC123_HSZF_2024251.Application.Services
{
    public class DataImporterService : IDataImporterService
    {
        private readonly TaxiDbContext _context;

        public DataImporterService(TaxiDbContext context)
        {
            _context = context;
        }

        public async Task ImportDataAsync(string filePath)
        {
            var jsonData = await File.ReadAllTextAsync(filePath);
            var taxiCars = JsonSerializer.Deserialize<List<TaxiCar>>(jsonData);

            if (taxiCars == null)
                throw new Exception("Invalid JSON format.");

            foreach (var car in taxiCars)
            {
                var existingCar = await _context.TaxiCars
                    .Include(tc => tc.Fares)
                    .FirstOrDefaultAsync(tc => tc.LicensePlate == car.LicensePlate);

                if (existingCar != null)
                {
                    // Add only new fares
                    foreach (var fare in car.Fares)
                    {
                        if (!existingCar.Fares.Any(f => f.FareStartDate == fare.FareStartDate))
                        {
                            existingCar.Fares.Add(fare);
                        }
                    }
                }
                else
                {
                    // Add the entire car
                    await _context.TaxiCars.AddAsync(car);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
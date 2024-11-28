using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ABC123_HSZF_2024251.Model;
using ABC123_HSZF_2024251.Persistence.MsSql;
using ABC123_HSZF_2024251.Application.Interfaces;
using System.Text.Json.Serialization;
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

            // Deserialize to temporary DTO
            var taxiCarsWrapper = JsonSerializer.Deserialize<TempTaxiCarsWrapper>(jsonData);

            if (taxiCarsWrapper == null || taxiCarsWrapper.TaxiCars == null)
            {
                throw new Exception("Invalid JSON format or missing TaxiCars.");
            }

            foreach (var tempCar in taxiCarsWrapper.TaxiCars)
            {
                var existingCar = await _context.TaxiCars
                    .Include(tc => tc.Fares)
                    .FirstOrDefaultAsync(tc => tc.LicensePlate == tempCar.LicensePlate);

                // Összegyűjtjük az összes fuvart (Fares és Services is)
                var allFares = (tempCar.Fares ?? new List<Fare>())
                    .Concat(tempCar.Services ?? new List<Fare>())
                    .ToList();

                if (existingCar != null)
                {
                    // Add only new fares
                    foreach (var fare in allFares)
                    {
                        if (!existingCar.Fares.Any(f => f.FareStartDate == fare.FareStartDate))
                        {
                            existingCar.Fares.Add(fare);
                        }
                    }
                }
                else
                {
                    // Create new TaxiCar with all fares
                    var newCar = new TaxiCar
                    {
                        LicensePlate = tempCar.LicensePlate,
                        Driver = tempCar.Driver,
                        Fares = allFares
                    };

                    await _context.TaxiCars.AddAsync(newCar);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

public class TempTaxiCarsWrapper
{
    public List<TempTaxiCarData> TaxiCars { get; set; }
}

public class TempTaxiCarData
{
    public string LicensePlate { get; set; }
    public string Driver { get; set; }

    [JsonPropertyName("Fares")]
    public List<Fare> Fares { get; set; }

    [JsonPropertyName("Services")]
    public List<Fare> Services { get; set; }
}

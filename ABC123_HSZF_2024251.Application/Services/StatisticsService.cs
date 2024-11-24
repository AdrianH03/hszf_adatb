using ABC123_HSZF_2024251.Application.Interfaces;
using ABC123_HSZF_2024251.Model;
using ABC123_HSZF_2024251.Persistence.MsSql;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ABC123_HSZF_2024251.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly TaxiDbContext _context;

        public StatisticsService(TaxiDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> GetShortTripsCountAsync()
        {
            return await _context.TaxiCars
                .ToDictionaryAsync(
                    car => car.LicensePlate,
                    car => car.Fares.Count(fare => fare.Distance < 10)
                );
        }

        public async Task<Dictionary<string, double>> GetAverageDistanceAsync()
        {
            return await _context.TaxiCars
                .ToDictionaryAsync(
                    car => car.LicensePlate,
                    car => car.Fares.Average(fare => fare.Distance)
                );
        }

        public async Task<Dictionary<string, Fare>> GetLongestAndShortestTripAsync()
        {
            return await _context.TaxiCars
                .ToDictionaryAsync(
                    car => car.LicensePlate,
                    car => new Fare
                    {
                        From = car.Fares.OrderBy(f => f.Distance).FirstOrDefault()?.From,
                        To = car.Fares.OrderBy(f => f.Distance).FirstOrDefault()?.To,
                        Distance = car.Fares.OrderBy(f => f.Distance).FirstOrDefault()?.Distance ?? 0,
                        PaidAmount = car.Fares.OrderBy(f => f.Distance).FirstOrDefault()?.PaidAmount ?? 0
                    }
                );
        }

        public async Task<Dictionary<string, string>> GetMostCommonDestinationAsync()
        {
            return await _context.TaxiCars
                .ToDictionaryAsync(
                    car => car.LicensePlate,
                    car => car.Fares
                        .GroupBy(f => f.To)
                        .OrderByDescending(g => g.Count())
                        .FirstOrDefault()?.Key
                );
        }
        public async Task GenerateStatisticsAsync()
        {
            var shortTripsCount = await GetShortTripsCountAsync();
            var averageDistance = await GetAverageDistanceAsync();
            var longestAndShortestTrips = await GetLongestAndShortestTripAsync();
            var mostCommonDestinations = await GetMostCommonDestinationAsync();

            StringBuilder sb = new StringBuilder();

            // Kiírás a konzolra vagy fájlba
            sb.AppendLine("Short Trips Count (< 10 km):");
            foreach (var entry in shortTripsCount)
            {
                sb.AppendLine($"{entry.Key}: {entry.Value}");
            }

            sb.AppendLine("\nAverage Distance:");
            foreach (var entry in averageDistance)
            {
                sb.AppendLine($"{entry.Key}: {entry.Value:F2} km");
            }

            sb.AppendLine("\nLongest and Shortest Trips:");
            foreach (var entry in longestAndShortestTrips)
            {
                (Fare longestTrip, Fare shortestTrip) = entry.Value;
                sb.AppendLine($"{entry.Key}: Longest trip: {longestTrip?.Distance} km, Shortest trip: {shortestTrip?.Distance} km");
            }

            sb.AppendLine("\nMost Common Destination:");
            foreach (var entry in mostCommonDestinations)
            {
                sb.AppendLine($"{entry.Key}: {entry.Value}");
            }

            // Például fájlba mentés
            await File.WriteAllTextAsync("TaxiStatistics.txt", sb.ToString());

            // Vagy kiírás a konzolra (opcionális)
            Console.WriteLine(sb.ToString());
        }
    }
}

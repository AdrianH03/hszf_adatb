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
            var cars = await _context.TaxiCars.ToListAsync();
            return cars.ToDictionary(
                car => car.LicensePlate,
                car => car.Fares.Count(fare => fare.Distance < 10)
            );
        }

        public async Task<Dictionary<string, double>> GetAverageDistanceAsync()
        {
            var cars = await _context.TaxiCars
                .Where(car => !string.IsNullOrWhiteSpace(car.LicensePlate))
                .ToListAsync();

            return cars.ToDictionary(
                car => car.LicensePlate,
                car => car.Fares.Any(fare => fare.Distance >= 0)
                    ? car.Fares.Where(fare => fare.Distance >= 0).Average(fare => fare.Distance)
                    : 0
            );
        }



        public async Task<Dictionary<string, (Fare LongestTrip, Fare ShortestTrip)>> GetLongestAndShortestTripAsync()
        {
            return await _context.TaxiCars
                .Include(car => car.Fares) // Explicit loading a Fares entitások
                .ToDictionaryAsync(
                    car => car.LicensePlate,
                    car => (car.Fares.OrderByDescending(f => f.Distance).FirstOrDefault(),
                            car.Fares.OrderBy(f => f.Distance).FirstOrDefault()));
        }

        public async Task<Dictionary<string, string>> GetMostCommonDestinationAsync()
        {
            return await _context.TaxiCars
                .Include(car => car.Fares) // Explicit loading a Fares entitások
                .ToDictionaryAsync(
                    car => car.LicensePlate,
                    car => car.Fares.GroupBy(f => f.To)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault());
        }
        public async Task GenerateStatisticsAsync()
        {
            var shortTripsCount = await GetShortTripsCountAsync();
            var averageDistance = await GetAverageDistanceAsync();
            var longestAndShortestTrips = await GetLongestAndShortestTripAsync();
            var mostCommonDestinations = await GetMostCommonDestinationAsync();

            StringBuilder sb = new StringBuilder();

            // Short trips
            sb.AppendLine("Short Trips Count (< 10 km):");
            foreach (var entry in shortTripsCount)
            {
                sb.AppendLine($"{entry.Key}: {entry.Value}");
            }

            // Average distance
            sb.AppendLine("\nAverage Distance:");
            foreach (var entry in averageDistance)
            {
                sb.AppendLine($"{entry.Key}: {entry.Value:F2} km");
            }

            // Longest and shortest trips
            sb.AppendLine("\nLongest and Shortest Trips:");
            foreach (var entry in longestAndShortestTrips)
            {
                // Dekonstruálás explicit tuple típusokkal
                var (longestTrip, shortestTrip) = entry.Value;

                sb.AppendLine($"{entry.Key}: Longest trip: {longestTrip?.Distance} km, Shortest trip: {shortestTrip?.Distance} km");
            }

            // Most common destination
            sb.AppendLine("\nMost Common Destination:");
            foreach (var entry in mostCommonDestinations)
            {
                sb.AppendLine($"{entry.Key}: {entry.Value}");
            }

            // Write to file
            await File.WriteAllTextAsync("TaxiStatistics.txt", sb.ToString());

            // Output to console
            Console.WriteLine(sb.ToString());
        }

    }
}

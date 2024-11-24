using ABC123_HSZF_2024251.Model;

namespace ABC123_HSZF_2024251.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<Dictionary<string, int>> GetShortTripsCountAsync();
        Task<Dictionary<string, double>> GetAverageDistanceAsync();
        Task<Dictionary<string, Fare>> GetLongestAndShortestTripAsync();
        Task<Dictionary<string, string>> GetMostCommonDestinationAsync();
        Task GenerateStatisticsAsync(); // Az új metódus
    }
}

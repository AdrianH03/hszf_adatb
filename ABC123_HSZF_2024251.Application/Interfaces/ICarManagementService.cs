using ABC123_HSZF_2024251.Model;

namespace ABC123_HSZF_2024251.Application.Interfaces
{
    public interface ICarManagementService
    {
        Task AddCarAsync(TaxiCar car);
        Task UpdateCarAsync(TaxiCar car);
        Task DeleteCarAsync(string licensePlate);
        Task AddFareAsync(string licensePlate, Fare fare);
        Task<List<TaxiCar>> GetCarsAsync();  // Az összes autó lekérdezésére szolgáló metódus

    }
}

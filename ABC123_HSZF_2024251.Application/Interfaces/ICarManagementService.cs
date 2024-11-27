using ABC123_HSZF_2024251.Model;

namespace ABC123_HSZF_2024251.Application.Interfaces
{
    public interface ICarManagementService
    {
        Task AddCarAsync(TaxiCar car);
        Task UpdateCarAsync(TaxiCar car);
        Task DeleteCarAsync(string licensePlate);
        Task AddFareAsync(string licensePlate, Fare fare, Action<string> notification);
        Task<List<TaxiCar>> GetCarsAsync();  // Az összes autó lekérdezésére szolgáló metódus
        Task<TaxiCar?> GetCarByLicensePlateAsync(string licensePlate);
        Task<List<TaxiCar>> SearchCarsAsync(string licensePlate, string driver);

    }
}

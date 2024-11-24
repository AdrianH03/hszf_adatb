using System.Collections.Generic;
using System.Threading.Tasks;
using ABC123_HSZF_2024251.Model;
namespace ABC123_HSZF_2024251.Application.Interfaces
{
    public interface ITaxiService
    {
        Task<List<TaxiCar>> GetAllCarsAsync();
        Task<TaxiCar?> GetCarByLicensePlateAsync(string licensePlate);
        Task AddCarAsync(TaxiCar car);
        Task UpdateCarAsync(TaxiCar car);
        Task DeleteCarAsync(string licensePlate);
        Task AddFareToCarAsync(string licensePlate, Fare fare);
    }
}

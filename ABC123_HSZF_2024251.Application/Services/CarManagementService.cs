using ABC123_HSZF_2024251.Application.Interfaces;
using ABC123_HSZF_2024251.Model;
using ABC123_HSZF_2024251.Persistence.MsSql;
using Microsoft.EntityFrameworkCore;

namespace ABC123_HSZF_2024251.Application.Services
{
    public class CarManagementService : ICarManagementService
    {
        private readonly TaxiDbContext _context;

        public CarManagementService(TaxiDbContext context)
        {
            _context = context;
        }

        public async Task AddCarAsync(TaxiCar car)
        {
            _context.TaxiCars.Add(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(TaxiCar car)
        {
            _context.TaxiCars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(string licensePlate)
        {
            var car = await _context.TaxiCars
                .FirstOrDefaultAsync(tc => tc.LicensePlate == licensePlate);

            if (car != null)
            {
                _context.TaxiCars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddFareAsync(string licensePlate, Fare fare)
        {
            var car = await _context.TaxiCars
                .FirstOrDefaultAsync(tc => tc.LicensePlate == licensePlate);

            if (car != null)
            {
                car.Fares.Add(fare);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<TaxiCar>> GetCarsAsync()
        {
            // Az összes autót és azok utazásait is lekérdezzük
            return await _context.TaxiCars
                .Include(tc => tc.Fares)
                .ToListAsync();
        }
    }
}

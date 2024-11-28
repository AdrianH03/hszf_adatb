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

        public async Task AddFareAsync(string licensePlate, Fare fare, Action<string> notification)
        {
            var car = await _context.TaxiCars
                .FirstOrDefaultAsync(tc => tc.LicensePlate == licensePlate);

            if (car == null)
            {
                throw new ArgumentException($"A megadott rendszámú autó ({licensePlate}) nem található.");
            }

            // A Fares property automatikusan betöltődik a lazy loading miatt
            var maxPaidAmount = car.Fares.Any()
                ? car.Fares.Max(f => f.PaidAmount)
                : 0;

            if (fare.PaidAmount > 2 * maxPaidAmount && maxPaidAmount > 0)
            {
                notification?.Invoke($"Figyelem! Az új út ({fare.PaidAmount} Ft) többe került, mint az autó bármelyik eddigi útjának kétszerese ({maxPaidAmount * 2} Ft).");
            }

            car.Fares.Add(fare);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TaxiCar>> GetCarsAsync()
        {
            return await _context.TaxiCars.ToListAsync();
        }

        public async Task<List<TaxiCar>> SearchCarsAsync(string? licensePlate = null, string? driver = null)
        {
            // Mivel van lazy loading, nem kell Include
            var query = _context.TaxiCars.AsQueryable();

            if (!string.IsNullOrWhiteSpace(licensePlate))
            {
                var lowerLicensePlate = licensePlate.ToLower();
                query = query.Where(car => car.LicensePlate.ToLower().Contains(lowerLicensePlate));
            }

            if (!string.IsNullOrWhiteSpace(driver))
            {
                var lowerDriver = driver.ToLower();
                query = query.Where(car => car.Driver.ToLower().Contains(lowerDriver));
            }

            return await query.ToListAsync();
        }

        public async Task<TaxiCar?> GetCarByLicensePlateAsync(string licensePlate)
        {
            return await _context.TaxiCars.FirstOrDefaultAsync(c => c.LicensePlate == licensePlate);
        }
    }
}

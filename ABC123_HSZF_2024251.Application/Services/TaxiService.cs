using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABC123_HSZF_2024251.Model;
using ABC123_HSZF_2024251.Persistence;
using ABC123_HSZF_2024251.Persistence.MsSql;
using Microsoft.EntityFrameworkCore;
using ABC123_HSZF_2024251.Application.Interfaces;
namespace ABC123_HSZF_2024251.Application.Services
{
    public class TaxiService : ITaxiService
    {
        private readonly TaxiDbContext _context;

        public TaxiService(TaxiDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaxiCar>> GetAllCarsAsync()
        {
            return await _context.TaxiCars.Include(tc => tc.Fares).ToListAsync();
        }

        public async Task<TaxiCar?> GetCarByLicensePlateAsync(string licensePlate)
        {
            return await _context.TaxiCars
                .Include(tc => tc.Fares)
                .FirstOrDefaultAsync(tc => tc.LicensePlate == licensePlate);
        }

        public async Task AddCarAsync(TaxiCar car)
        {
            await _context.TaxiCars.AddAsync(car);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(TaxiCar car)
        {
            _context.TaxiCars.Update(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(string licensePlate)
        {
            var car = await GetCarByLicensePlateAsync(licensePlate);
            if (car != null)
            {
                _context.TaxiCars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddFareToCarAsync(string licensePlate, Fare fare)
        {
            var car = await GetCarByLicensePlateAsync(licensePlate);
            if (car == null)
                throw new Exception("Car not found.");

            if (car.Fares.Any(f => f.FareStartDate == fare.FareStartDate))
                throw new Exception("Fare already exists.");

            if (fare.PaidAmount > car.Fares.Max(f => f.PaidAmount) * 2)
            {
                Console.WriteLine($"Warning: Fare cost exceeds double the maximum paid amount for this car!");
            }

            car.Fares.Add(fare);
            await _context.SaveChangesAsync();
        }
    }
}

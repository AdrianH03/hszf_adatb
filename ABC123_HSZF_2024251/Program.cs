﻿using ABC123_HSZF_2024251.Application.Services;
using ABC123_HSZF_2024251.Application.Interfaces;
using ABC123_HSZF_2024251.Persistence.MsSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ABC123_HSZF_2024251.Model;

class Program
{
    static async Task Main(string[] args)
    {
        // Host konfigurálása
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // DbContext regisztráció
                services.AddDbContext<TaxiDbContext>(options =>
                    options.UseSqlite("Data Source=TaxiDatabase.db"));

                // Szolgáltatások regisztrációja
                services.AddScoped<IDataImporterService, DataImporterService>();
                services.AddScoped<IStatisticsService, StatisticsService>();
                services.AddScoped<ICarManagementService, CarManagementService>();
            })
            .Build();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            // A program belépési pontja
            await RunApplicationAsync(services);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task RunApplicationAsync(IServiceProvider services)
    {
        var dataImporter = services.GetRequiredService<IDataImporterService>();
        var carManager = services.GetRequiredService<ICarManagementService>();
        var statisticsService = services.GetRequiredService<IStatisticsService>();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("Taxi Management System");
            Console.WriteLine("1. JSON fájl betöltése");
            Console.WriteLine("2. Autók listázása");
            Console.WriteLine("3. Új autó hozzáadása");
            Console.WriteLine("4. Autó módosítása");
            Console.WriteLine("5. Autó törlése");
            Console.WriteLine("6. Új út hozzáadása");
            Console.WriteLine("7. Statisztikák generálása");
            Console.WriteLine("8. Kilépés");
            Console.Write("Válassz egy opciót: ");

            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    await ImportJsonAsync(dataImporter);
                    break;
                case "2":
                    await ListCarsAsync(carManager);
                    break;
                case "3":
                    await AddCarAsync(carManager);
                    break;
                case "4":
                    await UpdateCarAsync(carManager);
                    break;
                case "5":
                    await DeleteCarAsync(carManager);
                    break;
                case "6":
                    await AddFareAsync(carManager);
                    break;
                case "7":
                    //await GenerateStatisticsAsync(statisticsService);
                    break;
                case "8":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Érvénytelen opció, próbáld újra.");
                    break;
            }
        }
    }

    // Implementáció az egyes menüpontokhoz
    static async Task ImportJsonAsync(IDataImporterService dataImporter)
    {
        Console.Write("Add meg a JSON fájl elérési útvonalát: ");
        var filePath = Console.ReadLine();

        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        {
            Console.WriteLine("Érvénytelen fájl elérési útvonal.");
            return;
        }

        try
        {
            await dataImporter.ImportDataAsync(filePath);
            Console.WriteLine("Adatok sikeresen betöltve.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt: {ex.Message}");
        }
    }

    static async Task ListCarsAsync(ICarManagementService carManager)
    {
        var cars = await carManager.GetCarsAsync();
        foreach (var car in cars)
        {
            Console.WriteLine($"Rendszám: {car.LicensePlate}, Sofőr: {car.Driver}");
        }
    }

    static async Task AddCarAsync(ICarManagementService carManager)
    {
        Console.Write("Add meg az autó rendszámát: ");
        var licensePlate = Console.ReadLine();

        Console.Write("Add meg a sofőr nevét: ");
        var driver = Console.ReadLine();

        await carManager.AddCarAsync(new TaxiCar
        {
            LicensePlate = licensePlate,
            Driver = driver
        });

        Console.WriteLine("Az autó hozzáadva.");
    }

    static async Task UpdateCarAsync(ICarManagementService carManager)
    {
        Console.Write("Add meg a módosítandó autó rendszámát: ");
        var licensePlate = Console.ReadLine();

        Console.Write("Add meg az új sofőr nevét: ");
        var newDriver = Console.ReadLine();

        TaxiCar car = new TaxiCar();
        car.LicensePlate = licensePlate;
        car.Driver = newDriver;
        await carManager.UpdateCarAsync(car);
        Console.WriteLine("Az autó frissítve.");
    }

    static async Task DeleteCarAsync(ICarManagementService carManager)
    {
        Console.Write("Add meg a törölni kívánt autó rendszámát: ");
        var licensePlate = Console.ReadLine();

        await carManager.DeleteCarAsync(licensePlate);
        Console.WriteLine("Az autó törölve.");
    }

    static async Task AddFareAsync(ICarManagementService carManager)
    {
        Console.Write("Add meg az autó rendszámát: ");
        var licensePlate = Console.ReadLine();

        Console.Write("Add meg az indulási helyet: ");
        var from = Console.ReadLine();

        Console.Write("Add meg a célállomást: ");
        var to = Console.ReadLine();

        Console.Write("Add meg a távolságot (km): ");
        var distance = double.Parse(Console.ReadLine());

        Console.Write("Add meg a viteldíjat: ");
        var paidAmount = decimal.Parse(Console.ReadLine());

        await carManager.AddFareAsync(licensePlate, new Fare
        {
            From = from,
            To = to,
            Distance = distance,
            PaidAmount = paidAmount,
            FareStartDate = DateTime.UtcNow
        });

        Console.WriteLine("Az út hozzáadva.");
    }

    /*static async Task GenerateStatisticsAsync(IStatisticsService statisticsService)
    {
        Console.Write("Add meg a statisztika fájl elérési útvonalát (üresen hagyva az alapértelmezettet használja): ");
        var filePath = Console.ReadLine();

        await statisticsService.GenerateStatisticsAsync(filePath);
        Console.WriteLine("Statisztika generálva.");
    }*/
}

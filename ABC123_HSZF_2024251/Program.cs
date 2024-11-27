using ABC123_HSZF_2024251.Application.Services;
using ABC123_HSZF_2024251.Application.Interfaces;
using ABC123_HSZF_2024251.Persistence.MsSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ABC123_HSZF_2024251.Model;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        // Host konfigurálása
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                // appsettings.json betöltése
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                // Naplózási szintek beállítása
                logging.ClearProviders(); // Alapértelmezett naplózók törlése
                logging.AddConsole(); // Konzolos naplózás
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging")); // appsettings.json alkalmazása
            })
            .ConfigureServices((context, services) =>
            {
                // DbContext regisztráció SQLite-tal
                services.AddDbContext<TaxiDbContext>(options =>
                    options.UseSqlite("Data Source=TaxiDatabase.db",
                    sqliteOptions => sqliteOptions.MigrationsAssembly("ABC123_HSZF_2024251.Persistence.MsSql"))
                    .LogTo(Console.WriteLine, LogLevel.Warning) // Csak figyelmeztetések és hibák
                );

                // Szolgáltatások regisztrációja
                services.AddScoped<IDataImporterService, DataImporterService>();
                services.AddScoped<IStatisticsService, StatisticsService>();
                services.AddScoped<ICarManagementService, CarManagementService>();
            })
            .Build();

        // Migrációk alkalmazása és inicializálás
        using (var scope = host.Services.CreateScope())
        {
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TaxiDbContext>();

                // Migrációk lefuttatása, ha szükséges
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hiba az adatbázis inicializálása során: {ex.Message}");
            }
        }

        // Program futtatása
        await RunApplicationAsync(host.Services);
    }

    static async Task RunApplicationAsync(IServiceProvider services)
    {
        var dataImporter = services.GetRequiredService<IDataImporterService>();
        var carManager = services.GetRequiredService<ICarManagementService>();
        var statisticsService = services.GetRequiredService<IStatisticsService>();

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n------------------------------------");
            Console.WriteLine("Taxi Management System");
            Console.WriteLine("1. JSON fájl betöltése");
            Console.WriteLine("2. Autók listázása");
            Console.WriteLine("3. Új autó hozzáadása");
            Console.WriteLine("4. Autó módosítása");
            Console.WriteLine("5. Autó törlése");
            Console.WriteLine("6. Új út hozzáadása");
            Console.WriteLine("7. Statisztikák generálása");
            Console.WriteLine("8. Kilépés");
            Console.WriteLine("------------------------------------");
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
                    await GenerateStatisticsAsync(statisticsService);
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
        catch (FileNotFoundException)
        {
            Console.WriteLine("A megadott fájl nem található.");
        }
        catch (JsonException)
        {
            Console.WriteLine("Hiba a JSON fájl feldolgozása során. Ellenőrizd a fájl formátumát.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ismeretlen hiba történt: {ex.Message}");
        }
    }
    


    static async Task ListCarsAsync(ICarManagementService carManager)
    {
        Console.WriteLine("\nKeresés autók között:");
        Console.WriteLine("Adja meg a keresési paramétereket (Enter, ha nem ad meg semmit):");

        Console.Write("Rendszám (vagy annak egy része): ");
        var licensePlate = Console.ReadLine();

        Console.Write("Sofőr neve (vagy annak egy része): ");
        var driver = Console.ReadLine();

        try
        {
            // Keresés a megadott paraméterekkel
            var cars = await carManager.SearchCarsAsync(licensePlate, driver);

            // Az eredmények megjelenítése
            if (cars.Any())
            {
                Console.WriteLine("\nTalált autók:");
                foreach (var car in cars)
                {
                    Console.WriteLine($"Rendszám: {car.LicensePlate}, Sofőr: {car.Driver}");
                    if (car.Fares.Any())
                    {
                        Console.WriteLine("  Viteldíjak:");
                        foreach (var fare in car.Fares)
                        {
                            Console.WriteLine($"    {fare.From} -> {fare.To}, Távolság: {fare.Distance} km, Díj: {fare.PaidAmount} Ft");
                        }
                    }
                    else
                    {
                        Console.WriteLine("  Nincs viteldíj.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Nem található a keresésnek megfelelő autó.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt a keresés során: {ex.Message}");
        }
    }


    static async Task AddCarAsync(ICarManagementService carManager)
    {
        Console.Write("Add meg az autó rendszámát: ");
        var licensePlate = Console.ReadLine();

        Console.Write("Add meg a sofőr nevét: ");
        var driver = Console.ReadLine();

        if (string.IsNullOrEmpty(licensePlate) || string.IsNullOrEmpty(driver))
        {
            Console.WriteLine("A rendszám és a sofőr megadása kötelező.");
            return;
        }

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

        var car = await carManager.GetCarByLicensePlateAsync(licensePlate);
        if (car == null)
        {
            Console.WriteLine("A megadott autó nem található.");
            return;
        }

        Console.Write("Add meg az új sofőr nevét: ");
        var newDriver = Console.ReadLine();

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

        try
        {
            await carManager.AddFareAsync(licensePlate, new Fare
            {
                From = from,
                To = to,
                Distance = distance,
                PaidAmount = paidAmount,
                FareStartDate = DateTime.UtcNow
            }, message =>
            {
                Console.WriteLine(message); // Eseménykezelő: értesítés megjelenítése a konzolon
            });
        }catch(Exception ex)
        {
            Console.WriteLine($"Hiba történt: {ex.Message}");
        }

        Console.WriteLine("Az út hozzáadva.");
    }
    static async Task GenerateStatisticsAsync(IStatisticsService statisticsService)
    {
        try
        {
            await statisticsService.GenerateStatisticsAsync();
            Console.WriteLine("Statisztika generálva és mentve a TaxiStatistics.txt fájlba.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba történt a statisztikák generálása során: {ex.Message}");
        }
    }

}

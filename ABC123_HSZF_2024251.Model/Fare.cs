using System;

namespace ABC123_HSZF_2024251.Model
{
    public class Fare
    {
        public int Id { get; set; } // Primary Key
        public string From { get; set; } = string.Empty; // Kiindulópont
        public string To { get; set; } = string.Empty; // Úti cél
        public double Distance { get; set; } // Távolság (km-ben)
        public decimal PaidAmount { get; set; } // Fizetett összeg
        public DateTime FareStartDate { get; set; } // Indulási idő

        // Kapcsolódó TaxiCar azonosítója
        public int TaxiCarId { get; set; } // Foreign Key
        public TaxiCar TaxiCar { get; set; } = null!; // Navigációs tulajdonság
    }
}

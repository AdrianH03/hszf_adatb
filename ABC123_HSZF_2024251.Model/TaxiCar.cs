using System.Collections.Generic;

namespace ABC123_HSZF_2024251.Model
{
    public class TaxiCar
    {
        public int Id { get; set; } // Primary Key
        public string LicensePlate { get; set; } = string.Empty; // Egyedi azonosító
        public string Driver { get; set; } = string.Empty; // Sofőr neve

        // Navigációs tulajdonságok
        public ICollection<Fare> Fares { get; set; } = new List<Fare>();
    }
}

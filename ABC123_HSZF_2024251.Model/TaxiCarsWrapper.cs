using System.Collections.Generic;

namespace ABC123_HSZF_2024251.Model
{
    public class TaxiCarsWrapper
    {
        public ICollection<TaxiCar> TaxiCars { get; set; } = new List<TaxiCar>();
    }
}

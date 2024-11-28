using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace ABC123_HSZF_2024251.Model
{
    public class Fare
    {
        private ILazyLoader _lazyLoader;
        private TaxiCar _car;

        public Fare()
        {
        }

        public Fare(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public double Distance { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime FareStartDate { get; set; }

        public int TaxiCarId { get; set; } // Idegen kulcs
        public virtual TaxiCar Car
        {
            get => _lazyLoader.Load(this, ref _car);
            set => _car = value;
        }
    }
}

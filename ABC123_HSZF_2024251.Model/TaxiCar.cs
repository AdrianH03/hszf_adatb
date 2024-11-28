using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace ABC123_HSZF_2024251.Model
{
    public class TaxiCar
    {
        private ILazyLoader _lazyLoader;
        private ICollection<Fare> _fares;

        public TaxiCar()
        {
        }

        public TaxiCar(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        public int Id { get; set; }
        public string LicensePlate { get; set; }
        public string Driver { get; set; }

        public virtual ICollection<Fare> Fares
        {
            get => _lazyLoader.Load(this, ref _fares);
            set => _fares = value;
        }
        public IEnumerable<Fare> GetFaresOrServices()
        {
            if (this.GetType().GetProperty("Fares") != null)
            {
                return (IEnumerable<Fare>)this.GetType().GetProperty("Fares").GetValue(this);
            }
            else if (this.GetType().GetProperty("Services") != null)
            {
                return (IEnumerable<Fare>)this.GetType().GetProperty("Services").GetValue(this);
            }
            return Enumerable.Empty<Fare>();
        }
    }

}

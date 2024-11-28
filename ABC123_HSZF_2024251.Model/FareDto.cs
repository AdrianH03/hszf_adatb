using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABC123_HSZF_2024251.Model
{
    public class FareDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public int Distance { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime FareStartDate { get; set; }
    }
}

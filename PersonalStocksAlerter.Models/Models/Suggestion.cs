using System;
namespace HP.PersonalStocksAlerter.Models.Models
{
    public class Suggestion
    {
        public decimal LowLimit { get; set; }
        public decimal HighLimit { get; set; }
        public Suggestion()
        {
        }
    }
}

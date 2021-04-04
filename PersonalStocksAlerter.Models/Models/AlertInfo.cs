using System;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HP.PersonalStocksAlerter.Models.Models
{
    public class AlertInfo
    {
        public decimal HighLimit { get; set; }
        public decimal LowLimit { get; set; }
        public decimal ExceptedLowPercentage { get; set; }
        public decimal ExceptedHighPercentage { get; set; }
        public AlertInfo(decimal exceptedLowPercentage, decimal exceptedHighPercentage)
        {
            ExceptedLowPercentage = exceptedLowPercentage;
            ExceptedHighPercentage = exceptedHighPercentage;
        }
    }
}

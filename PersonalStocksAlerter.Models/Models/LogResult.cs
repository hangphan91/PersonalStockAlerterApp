using System;
namespace HP.PersonalStocksAlerter.Models.Models
{
    public class LogResult
    {
        public SuggestedAction SuggestedAction { get; set; }
        public string SuggestionMessage { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public decimal BuyingSuggestionLowLimit { get; set; }
        public decimal BuyingSuggestionHighLimit { get; set; }
        public decimal SellingSuggestionLowLimit { get; set; }
        public decimal SellingSuggestionHighLimit { get; set; }
        public decimal ExpectedLowPercent { get; set; }
        public decimal ExpectedHighPercent { get; set; }
        public DateTime PostedTS { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using HP.PersonalStocksAlerter.Models.Models;
using Skender.Stock.Indicators;

namespace HP.PersonalStocks.Mgr.Factories
{
    public class AlertFactory
    {
        public List<Quote> HistoricalQuotes { get; set; }
        public List<StdDevResult> StdDevResults { get; set; }
        public AlertInfo StdAlertLowLimitInfo { get; set; }
        public AlertInfo StdAlertHighLimitInfo { get; set; }
        public AlertInfo StickerAletHighLimittInfo { get; set; }
        public bool HasLowStdLimit { get; set; }
        public bool HasHighStdLimit { get; set; }
        public Calculator Calculator { get; set; }
        public SecondCalculator SecondCalculator { get; set; }
        public AlertFactory(List<StdDevResult> stdDevResults, List<Quote> historicalQuotes)
        {
            HistoricalQuotes = historicalQuotes;
            StdDevResults = stdDevResults;
            //Calculator = new Calculator(HistoricalQuotes, StdDevResults);
            SecondCalculator = new SecondCalculator(HistoricalQuotes, stdDevResults);
        }
        public SuggestedAction SendToBuyOrSellAlert()
        {
            if (HasLowStdLimit)
            {
                var toBuy = Calculator.ToBuySticker(StdAlertLowLimitInfo);
                return toBuy ? SuggestedAction.Buy : SuggestedAction.WaitToBuy;
            }else if (HasHighStdLimit)
            {
                var toSell = Calculator.ToSellSticker(StdAlertHighLimitInfo);
                return toSell ? SuggestedAction.Sell : SuggestedAction.WaitToSell;
            }
            return SuggestedAction.Wait;
        }
        public void SetLowLimits(AlertInfo stdAlertLowLimitInfo)
        {
            StdAlertLowLimitInfo = stdAlertLowLimitInfo;
            HasLowStdLimit = true;
        }
        public void SetHighLimits(AlertInfo stdAlertHighLimitInfo)
        {
            StdAlertHighLimitInfo = stdAlertHighLimitInfo;
            HasHighStdLimit = true;
        }
        public string GetSuggestion(AlertInfo stdAlertHighLimitInfo, AlertInfo stdAlertLowLimitInfo)
        {
            return Calculator.GetSuggestions(stdAlertHighLimitInfo, stdAlertLowLimitInfo);
        }

        internal string GetSecondSuggestion()
        {
            return SecondCalculator.GetSuggestions();
        }
    }
}

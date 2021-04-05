using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HP.PersonalStocks.Mgr.Factories;
using HP.PersonalStocks.Mgr.Helpers;
using HP.PersonalStocksAlerter.Models.Models;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using Trady.Analysis.Indicator;
using YahooFinanceApi;

namespace HP.PersonalStocks.Mgr
{
    public class AlertMgr
    {
        public List<Quote> HistoricalQuotes { get; set; }
        public List<StdDevResult> StdDevResults { get; set; }
        public string CurrentSticker { get; set; }
        public AlertFactory Factory { get; set; }
        public decimal ExceptedLowPercentage { get; set; }
        public decimal ExceptedHighPercentage { get; set; }
        public AlertMgr(string currentSticker, decimal exceptedLowPercentage = 10, decimal exceptedHighPercentage = 10)
        {
            CurrentSticker = currentSticker;
            ExceptedLowPercentage = exceptedLowPercentage;
            ExceptedHighPercentage = exceptedHighPercentage;
            GetQuoteAndStdIndicator();
            Factory = new AlertFactory(StdDevResults, HistoricalQuotes);
        }
        public AlertResult GetAlertResult()
        {
            var result = new AlertResult();
            try
            {
                result = new AlertResult
                {
                    SuggestionMessage = GetSuggestionForCurrentSticker(),
                    SuggestedAction = CheckForAlert(),
                    Success = true
                };
            }
            catch (Exception ex)
            {
                result =  new AlertResult
                {
                    Success = false,
                    ErrorMessage = "Failed to get Alert and Suggestion." + ex.Message
                };
            }finally
            {
                if (result.Success)
                {
                    var log = new LogResult
                    {
                        SuggestedAction = result.SuggestedAction,
                        ErrorMessage = result.ErrorMessage,
                        Success = result.Success,
                        SuggestionMessage = result.SuggestionMessage,
                        ExpectedHighPercent = ExceptedHighPercentage,
                        ExpectedLowPercent = ExceptedLowPercentage,
                        BuyingSuggestionHighLimit = Factory.Calculator.BuyingSuggestion.HighLimit,
                        BuyingSuggestionLowLimit = Factory.Calculator.BuyingSuggestion.LowLimit,
                        SellingSuggestionHighLimit = Factory.Calculator.SellingSuggestion.HighLimit,
                        SellingSuggestionLowLimit = Factory.Calculator.SellingSuggestion.LowLimit,
                        PostedTS = DateTime.Now
                    };
                    new WriteToText(log);
                }
            }
            return result;
        }
        private SuggestedAction CheckForAlert()
        {
            var currentPrice = HistoricalQuotes.OrderByDescending(q => q.Date).FirstOrDefault();
            if (currentPrice?.Close >= Factory.Calculator.SellingSuggestion.LowLimit)
                return SuggestedAction.Sell;
            else if (currentPrice?.Close < Factory.Calculator.SellingSuggestion.LowLimit && SendHighLimit())
                return SuggestedAction.WaitToSell;
            else if (Factory.Calculator.BuyingSuggestion.HighLimit >= currentPrice?.Close)
                return SuggestedAction.Buy;
            else if (Factory.Calculator.BuyingSuggestion.HighLimit < currentPrice?.Close && !SendHighLimit())
                return SuggestedAction.WaitToBuy;
            return SuggestedAction.Wait;

            //return Factory.SendToBuyOrSellAlert();
        }
        private string GetSuggestionForCurrentSticker()
        {
            GetQuoteAndStdIndicator();
            var stdAlertHighLimit = new AlertInfo(2, 10);
            var stdAlertlLowLimit = new AlertInfo(2, 10);
            var suggestionResult = Factory.GetSuggestion(stdAlertHighLimit, stdAlertlLowLimit);
            var suggestion = $" For Stock Symbol {CurrentSticker}: {suggestionResult}";
            return suggestion;
        }
        private void GetHistoricalQuotesInfoAsync()
        {
            try
            {
                var task = Task.Run(()=>
                Yahoo.GetHistoricalAsync( CurrentSticker, DateTime.Now.AddMonths(-12), DateTime.Now, Period.Daily));
                task.Wait();
                var historicalData = task.Result;
                HistoricalQuotes = new List<Quote>();
                foreach (var item in historicalData)
                {
                    HistoricalQuotes.Add(new Quote
                    {
                        Close = item.Close,
                        Open = item.Open,
                        Date = item.DateTime,
                        High = item.High,
                        Low = item.Low,
                        Volume= item.Volume
                    });
                }
                
            }
            catch (Exception ex)
            {
                //throw new Exception("Failed to Get Historical Quotes Info.", ex);
            }
        }
        private void GetQuoteAndStdIndicator()
        {
            GetHistoricalQuotesInfoAsync();
            StdDevResults = Indicator.GetStdDev(HistoricalQuotes,10).ToList();
        }
       
        private bool SendHighLimit()
        {
            var quoteAveragePrice = HistoricalQuotes.Average(s => s.Close);

            var currentQuotePrice = HistoricalQuotes
                .Where(q => q.Date.Date <= DateTime.Now.Date)
                .OrderByDescending(q => q.Date)
                .FirstOrDefault();
            var inHighRange = currentQuotePrice !=null && currentQuotePrice.Close >= quoteAveragePrice;
            return inHighRange;
        }
        
    }
}

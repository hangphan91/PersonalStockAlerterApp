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
        public Quote CurrentQuote { get; set; }
        public List<RsiResult> RsiResults { get; set; }
        public List<ChaikinOscResult> ChaikinOscResults { get; set; }
        public SuggestedAction FinalSuggestion { get; set; }
        public AlertMgr(string currentSticker)
        {
            CurrentSticker = currentSticker;
            GetQuoteAndStdIndicator();
            Factory = new AlertFactory(StdDevResults, HistoricalQuotes);
            CurrentQuote = HistoricalQuotes.OrderByDescending(q => q.Date).FirstOrDefault();
            RsiResults = new List<RsiResult>();
            ChaikinOscResults = new List<ChaikinOscResult>();
        }
        public AlertResult GetAlertResult()
        {
            var result = new AlertResult();
            try
            {

                result = new AlertResult
                {
                    SuggestionMessage = GetSuggestionForCurrentSticker(),
                    Success = true,
                    Symbol = CurrentSticker,
                    CurrentRSIValue = Factory.IndicatorSuggestions.GetRSIIndicator(RsiResults),
                    CurrentChaikinOSCValue = Factory.IndicatorSuggestions.GetChaikinOSCValue(ChaikinOscResults),
                    CurrentPrice = Math.Round(CurrentQuote.Close, 2)
                };
                var secondSuggestion = CheckForSecondAlert();
                result.SuggestedActions.Add(secondSuggestion.ToString());
                var indicatorSuggestions = CheckForIndicatorSuggestions();
                List<string> suggestions = GetStringSuggestions(indicatorSuggestions);
                result.SuggestedActions.AddRange(suggestions);
                FinalSuggestion = GetFinalSuggestion(indicatorSuggestions, secondSuggestion);
                result.FinalSuggestion = FinalSuggestion.ToString();
            }
            catch (Exception ex)
            {
                result = new AlertResult
                {
                    Success = false,
                    ErrorMessage = "Failed to get Alert and Suggestion." + ex.Message
                };
            }
            finally
            {
                if (result.Success)
                {
                    SuggestionMgr.SaveSuggestion(result);
                }
            }
            return result;
        }

        private SuggestedAction GetFinalSuggestion(List<SuggestedAction> indicatorSuggestions,
            SuggestedAction secondSuggestion)
        {
            var list = new List<SuggestedAction>
            {
                secondSuggestion
            };
            list.AddRange(indicatorSuggestions);
            var actions = new List<SuggestedAction>();
            foreach (var item in indicatorSuggestions)
            {
                switch (item)
                {
                    case SuggestedAction.OSCToBuy:
                    case SuggestedAction.RSIToBuy:
                        actions.Add(SuggestedAction.Buy);
                        break;
                    case SuggestedAction.RSIToWait:
                        actions.Add(SuggestedAction.Wait);
                        break;
                    case SuggestedAction.OSCToSell:
                    case SuggestedAction.RSIToSell:
                        actions.Add(SuggestedAction.Sell);
                        break;
                }
            }
            if (actions.All(a => a == SuggestedAction.Sell) &&
                secondSuggestion == SuggestedAction.BuyTheDip)
                return SuggestedAction.BuyTheDip;
            else if (actions.All(a => a == SuggestedAction.Buy) &&
                (secondSuggestion == SuggestedAction.Buy ||
                secondSuggestion == SuggestedAction.StrongBuy))
                return SuggestedAction.StrongBuy;
            else if (actions.Any(a => a == SuggestedAction.Buy)
                && actions.Any(a => a != SuggestedAction.Sell))
                return SuggestedAction.Buy;
            else if (actions.Any(a => a == SuggestedAction.Sell)
                 && actions.Any(a => a != SuggestedAction.Buy))
                return SuggestedAction.Sell;
            else
                return SuggestedAction.Wait;

        }

        private static List<string> GetStringSuggestions(List<SuggestedAction> indicatorSuggestions)
        {
            var suggestions = new List<string>();
            foreach (var item in indicatorSuggestions)
            {
                suggestions.Add(item.ToString());
            }

            return suggestions;
        }

        private List<SuggestedAction> CheckForIndicatorSuggestions()
        {
            var list = new List<SuggestedAction>();
            if (Factory.IndicatorSuggestions.OSCToBuy)
                list.Add(SuggestedAction.OSCToBuy);
            else
                list.Add(SuggestedAction.OSCToSell);
            if (Factory.IndicatorSuggestions.RSIToBuy)
                list.Add(SuggestedAction.RSIToBuy);
            else if (Factory.IndicatorSuggestions.RSIToSell)
                list.Add(SuggestedAction.RSIToSell);
            else
                list.Add(SuggestedAction.RSIToWait);

            return list;
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

        }
        private SuggestedAction CheckForSecondAlert()
        {
            var currentPrice = HistoricalQuotes.OrderByDescending(q => q.Date).FirstOrDefault();
            if (currentPrice?.Close <= Factory.SecondCalculator.SellingSuggestion.LowLimit)
                return SuggestedAction.BuyTheDip;
            else if (currentPrice?.Close >= Factory.SecondCalculator.SellingSuggestion.HighLimit)
                return SuggestedAction.Sell;
            else if (currentPrice?.Close >= Factory.SecondCalculator.HoldOrSellSuggestion.LowLimit &&
                currentPrice?.Close < Factory.SecondCalculator.HoldOrSellSuggestion.HighLimit)
                return SuggestedAction.HoldPriceCouldGoUp;
            else if (Factory.SecondCalculator.HoldOrBuySuggestion.HighLimit >= currentPrice?.Close &&
                currentPrice?.Close > Factory.SecondCalculator.HoldOrBuySuggestion.LowLimit)
                return SuggestedAction.HoldPriceCouldGoDown;
            else if (Factory.SecondCalculator.BuyingSuggestion.HighLimit >= currentPrice?.Close &&
                Factory.SecondCalculator.BuyingSuggestion.LowLimit < currentPrice?.Close)
            {
                if (currentPrice?.Close < Factory.SecondCalculator.FiveBasicNumber.Mean)
                    return SuggestedAction.StrongBuy;
                return SuggestedAction.Buy;
            }
            else return SuggestedAction.Wait;
        }
        private string GetSuggestionForCurrentSticker()
        {
            GetQuoteAndStdIndicator();
            var secondSuggestionResult = Factory.GetSecondSuggestion();
            var currentPrice = Math.Round(CurrentQuote.Close, 2);
            var suggestion = $"{CurrentSticker}'s Current Price: {currentPrice}." +
                $"Suggestion: {secondSuggestionResult}";
            return suggestion;
        }
        private void GetHistoricalQuotesInfoAsync()
        {
            try
            {
                var task = Task.Run(() =>
                Yahoo.GetHistoricalAsync(CurrentSticker, DateTime.Now.AddMonths(-12), DateTime.Now, Period.Daily));
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
                        Volume = item.Volume
                    });
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Get Historical Quotes Info. {ex.Message}");
            }
        }
        private void GetQuoteAndStdIndicator()
        {
            try
            {
                GetHistoricalQuotesInfoAsync();
                StdDevResults = Indicator.GetStdDev(HistoricalQuotes, 10).ToList();
                if (HistoricalQuotes.Count > 114)
                {
                    RsiResults = Indicator.GetRsi(HistoricalQuotes).ToList();
                    ChaikinOscResults = Indicator.GetChaikinOsc(HistoricalQuotes).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get Data");
            }
        }

        private bool SendHighLimit()
        {
            var quoteAveragePrice = HistoricalQuotes.Average(s => s.Close);

            var currentQuotePrice = HistoricalQuotes
                .Where(q => q.Date.Date <= DateTime.Now.Date)
                .OrderByDescending(q => q.Date)
                .FirstOrDefault();
            var inHighRange = currentQuotePrice != null && currentQuotePrice.Close >= quoteAveragePrice;
            return inHighRange;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using HP.PersonalStocksAlerter.Models.Models;
using Skender.Stock.Indicators;

namespace HP.PersonalStocks.Mgr.Factories
{
    public class Calculator
    {
        public List<Quote> HistoricalQuotes { get; set; }
        public List<StdDevResult> StdDevResults { get; set; }
        public bool ToAlertToBuy { get; set; }
        public bool ToAlertToSell { get; set; }
        public Suggestion BuyingSuggestion { get; set; }
        public Suggestion SellingSuggestion { get; set; }
        public Calculator(List<Quote> historicalQuotes, List<StdDevResult> stdDevResults)
        {
            HistoricalQuotes = historicalQuotes;
            StdDevResults = stdDevResults;
            BuyingSuggestion = new Suggestion();
            SellingSuggestion = new Suggestion();
        }
        public Calculator()
        {
            HistoricalQuotes = new List<Quote>();
            StdDevResults = new List<StdDevResult>();
        }
        public bool ToBuySticker(AlertInfo stdLowAlertInfo)
        {
            CaculateLowLimitResult(stdLowAlertInfo);
            return ToAlertToBuy;
        }
        public bool ToSellSticker(AlertInfo stdHighAlertInfo)
        {
            CaculateHighLimitResult(stdHighAlertInfo);
            return ToAlertToSell;
        }
        private void CaculateLowLimitResult(AlertInfo stdLowAlertInfo)
        {
            var maxStickerQuote = HistoricalQuotes.Max(s => s.High);
            var minStickerQuote = HistoricalQuotes.Min(s => s.Low);
            CalculateHighAndLowLimitPriceForLowAlertInfo(stdLowAlertInfo);
            if (minStickerQuote <= stdLowAlertInfo.HighLimit && stdLowAlertInfo.LowLimit < minStickerQuote)
            {
                ToAlertToBuy = true;
            }
        }

        private void CalculateHighAndLowLimitPriceForLowAlertInfo(AlertInfo stdLowAlertInfo)
        {
            var maxStdResult = StdDevResults.Max(s => s.StdDev);
            var maxStdItem = StdDevResults.Where(s => s.StdDev == maxStdResult).FirstOrDefault();
            var maxPrice = maxStdItem.StdDev * maxStdItem.ZScore + maxStdItem.Mean;

            var minStdResult = StdDevResults.Min(s => s.StdDev);
            var minStdItem = StdDevResults.Where(s => s.StdDev == minStdResult).FirstOrDefault();
            var minPrice = minStdItem.StdDev * minStdItem.ZScore + minStdItem.Mean;

            var priceResultRange = maxPrice.Value - minPrice.Value;
            var lowLimitPriceRange = priceResultRange * (stdLowAlertInfo.ExceptedLowPercentage / 100);

            stdLowAlertInfo.LowLimit = minPrice.Value;
            stdLowAlertInfo.HighLimit = lowLimitPriceRange + minPrice.Value;
        }

        private void CaculateHighLimitResult(AlertInfo stdHighAlertInfo)
        {
            var maxStickerQuote = HistoricalQuotes.Max(s => s.High);
            var minStickerQuote = HistoricalQuotes.Min(s => s.Low);
            CalculateHighAndLowLimitPriceForHighRangeAlertInfo(stdHighAlertInfo);
            if (maxStickerQuote <= stdHighAlertInfo.HighLimit && stdHighAlertInfo.LowLimit < maxStickerQuote)
            {
                ToAlertToSell = true;
            }
        }

        private void CalculateHighAndLowLimitPriceForHighRangeAlertInfo(AlertInfo stdHighAlertInfo)
        {
            var maxStdResult = StdDevResults.Max(s => s.StdDev);
            var maxStdItem = StdDevResults.Where(s => s.StdDev == maxStdResult).FirstOrDefault();
            var maxPrice = maxStdItem.StdDev * maxStdItem.ZScore + maxStdItem.Mean;

            var minStdResult = StdDevResults.Min(s => s.StdDev);
            var minStdItem = StdDevResults.Where(s => s.StdDev == minStdResult).FirstOrDefault();
            var minPrice = minStdItem.StdDev * minStdItem.ZScore + minStdItem.Mean;

            var priceResultRange = maxPrice.Value - minPrice.Value;
            var highLimitStdRange = priceResultRange * (stdHighAlertInfo.ExceptedHighPercentage / 100);

            stdHighAlertInfo.HighLimit = maxPrice.Value;
            stdHighAlertInfo.LowLimit = maxPrice.Value - highLimitStdRange;
        }

        private void CalculateSuggestionPrices(AlertInfo stdHighAlertInfo, AlertInfo stdLowAlertInfo)
        {
            CalculateHighAndLowLimitPriceForLowAlertInfo(stdLowAlertInfo);

            BuyingSuggestion = new Suggestion
            {
                HighLimit = stdLowAlertInfo.HighLimit,
                LowLimit = stdLowAlertInfo.LowLimit
            };
            CalculateHighAndLowLimitPriceForHighRangeAlertInfo(stdHighAlertInfo);
            SellingSuggestion = new Suggestion
            {
                HighLimit = stdHighAlertInfo.HighLimit,
                LowLimit = stdHighAlertInfo.LowLimit
            };
        }

        public string GetSuggestions(AlertInfo stdHighAlertInfo, AlertInfo stdLowAlertInfo)
        {
            CalculateSuggestionPrices(stdHighAlertInfo, stdLowAlertInfo);

            var buyLowLimitPrice = Math.Round(BuyingSuggestion.LowLimit, 2);
            var buyHighLimitPrice = Math.Round(BuyingSuggestion.HighLimit, 2);
            string suggestion = SuggestionString(buyLowLimitPrice, buyHighLimitPrice, "Buy");

            var sellLowLimitPrice = Math.Round(SellingSuggestion.LowLimit, 2);
            var sellHighLimitPrice = Math.Round(SellingSuggestion.HighLimit, 2);
            suggestion += SuggestionString(sellLowLimitPrice, sellHighLimitPrice, "Sell");

            return suggestion;
        }

        private static string SuggestionString(decimal buyLowLimitPrice,
            decimal buyHighLimitPrice, string action)
        {
            var correctOrder = buyHighLimitPrice > buyLowLimitPrice;
            if (correctOrder)
            {
                return $"Our Suggested Target Range To {action}: " +
                            $"Between {buyLowLimitPrice} And {buyHighLimitPrice}.";
            }
            else
            {
                return $" Our Suggested Target Range To {action}: " +
                             $"Between {buyHighLimitPrice} And {buyLowLimitPrice}.";
            }
            
        }
    }
}

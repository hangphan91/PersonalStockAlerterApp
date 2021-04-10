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
            var currentPrice = HistoricalQuotes.OrderByDescending(q => q.Date).FirstOrDefault();
            CalculateHighAndLowLimitPriceForLowAlertInfo(stdLowAlertInfo, maxStickerQuote, minStickerQuote);
            if (currentPrice?.Close <= stdLowAlertInfo.HighLimit)
            {
                ToAlertToBuy = true;
            }
        }

        private void CalculateHighAndLowLimitPriceForLowAlertInfo(AlertInfo stdLowAlertInfo,
            decimal maxStickerQuote, decimal minStickerQuote)
        {
            var maxStdResult = StdDevResults.Max(s => s.StdDev);
            var maxStdItem = StdDevResults.Where(s => s.StdDev == maxStdResult).FirstOrDefault();
            var maxPrice = maxStdItem.StdDev * maxStdItem.ZScore + maxStdItem.Mean;
            //maxPrice = Math.Max(maxPrice.Value, maxStickerQuote);

            var minStdResult = StdDevResults.Min(s => s.StdDev);
            var minStdItem = StdDevResults.Where(s => s.StdDev == minStdResult).FirstOrDefault();
            var minPrice = minStdItem.StdDev * minStdItem.ZScore + minStdItem.Mean;
            //minPrice = Math.Min(minPrice.Value, minStickerQuote);

            var priceResultRange = maxPrice.Value - minPrice.Value;
            var lowLimitPriceRange = priceResultRange * (stdLowAlertInfo.ExceptedLowPercentage / 100);

            stdLowAlertInfo.LowLimit = minPrice.Value;
            stdLowAlertInfo.HighLimit = lowLimitPriceRange + minPrice.Value;
        }


        private void CaculateHighLimitResult(AlertInfo stdHighAlertInfo)
        {
            var maxStickerQuote = HistoricalQuotes.Max(s => s.High);
            var minStickerQuote = HistoricalQuotes.Min(s => s.Low);
            var currentPrice = HistoricalQuotes.OrderByDescending(q => q.Date).FirstOrDefault();

            CalculateHighAndLowLimitPriceForHighRangeAlertInfo(stdHighAlertInfo,
                maxStickerQuote, minStickerQuote);
            if (stdHighAlertInfo.LowLimit < currentPrice?.Close)
            {
                ToAlertToSell = true;
            }
        }

        private void CalculateHighAndLowLimitPriceForHighRangeAlertInfo(AlertInfo stdHighAlertInfo,
            decimal maxStickerQuote, decimal minStickerQuote)
        {
            var maxStdResult = StdDevResults.Max(s => s.StdDev);
            var maxStdItem = StdDevResults.Where(s => s.StdDev == maxStdResult).FirstOrDefault();
            var maxPrice = maxStdItem.StdDev * maxStdItem.ZScore + maxStdItem.Mean;
            //maxPrice = Math.Max(maxPrice.Value, maxStickerQuote);

            var minStdResult = StdDevResults.Min(s => s.StdDev);
            var minStdItem = StdDevResults.Where(s => s.StdDev == minStdResult).FirstOrDefault();
            var minPrice = minStdItem.StdDev * minStdItem.ZScore + minStdItem.Mean;
            //minPrice = Math.Min(minPrice.Value, minStickerQuote);

            var priceResultRange = maxPrice.Value - minPrice.Value;
            var highLimitStdRange = priceResultRange * (stdHighAlertInfo.ExceptedHighPercentage / 100);

            stdHighAlertInfo.HighLimit = maxPrice.Value;
            stdHighAlertInfo.LowLimit = maxPrice.Value - highLimitStdRange;
        }

        private void CalculateSuggestionPrices(AlertInfo stdHighAlertInfo, AlertInfo stdLowAlertInfo)
        {
            var maxStickerQuote = HistoricalQuotes.Max(s => s.High);
            var minStickerQuote = HistoricalQuotes.Min(s => s.Low);
            CalculateHighAndLowLimitPriceForLowAlertInfo(stdLowAlertInfo,
                maxStickerQuote, minStickerQuote);
            CalculateHighAndLowLimitPriceForHighRangeAlertInfo(stdHighAlertInfo,
                maxStickerQuote, maxStickerQuote);
            if(stdHighAlertInfo.HighLimit > stdLowAlertInfo.HighLimit)
                MakeSuggestions(stdHighAlertInfo, stdLowAlertInfo);
            else
                MakeSuggestions(stdLowAlertInfo, stdHighAlertInfo);

        }

        private void MakeSuggestions(AlertInfo stdHighAlertInfo, AlertInfo stdLowAlertInfo)
        {
            BuyingSuggestion = new Suggestion
            {
                HighLimit = stdLowAlertInfo.HighLimit,
                LowLimit = stdLowAlertInfo.LowLimit
            };
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

        private string SuggestionString(decimal lowLimitPrice,
            decimal highLimitPrice, string action)
        {
            var correctOrder = highLimitPrice > lowLimitPrice;
           
            if (correctOrder)
            {
                ConsilidateSuggestions(lowLimitPrice, highLimitPrice, action);
                return $"Our Suggested Target Range To {action}: " +
                            $"Between {lowLimitPrice} And {highLimitPrice}. ";
            }
            else
            {
                ConsilidateSuggestions(highLimitPrice, lowLimitPrice, action);
                return $"Our Suggested Target Range To {action}: " +
                             $"Between {highLimitPrice} And {lowLimitPrice}. ";
            }

        }

        private void ConsilidateSuggestions(decimal lowLimitPrice, decimal highLimitPrice, string action)
        {
            switch (action)
            {
                case "Buy":
                    BuyingSuggestion.LowLimit = lowLimitPrice;
                    BuyingSuggestion.HighLimit = highLimitPrice;
                    break;
                case "Sell":
                    SellingSuggestion.LowLimit = lowLimitPrice;
                    SellingSuggestion.HighLimit = highLimitPrice;
                    break;
                default:
                    break;
            }
        }
    }
}

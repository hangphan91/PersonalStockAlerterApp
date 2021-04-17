using System;
using System.Collections.Generic;
using System.Linq;
using HP.PersonalStocksAlerter.Models.Models;
using Skender.Stock.Indicators;

namespace HP.PersonalStocks.Mgr.Factories
{
    public class SecondCalculator
    {
        public List<Quote> HistoricalQuotes { get; set; }
        public List<StdDevResult> StdDevResults { get; set; }
        public bool ToAlertToBuy { get; set; }
        public bool ToAlertToSell { get; set; }
        public Suggestion BuyingSuggestion { get; set; }
        public Suggestion SellingSuggestion { get; set; }
        public Suggestion HoldOrSellSuggestion { get; set; }
        public Suggestion HoldOrBuySuggestion { get; set; }
        public SuggestedAction SuggestedAction { get; set; }
        public FiveBasicNumberValue FiveBasicNumber { get; set; }
        public SecondCalculator(List<Quote> historicalQuotes, List<StdDevResult> stdDevResults)
        {
            HistoricalQuotes = historicalQuotes;
            StdDevResults = stdDevResults;
            BuyingSuggestion = new Suggestion();
            SellingSuggestion = new Suggestion();
            HoldOrSellSuggestion = new Suggestion();
            FiveBasicNumber = new FiveBasicNumberValue();
        }
        public SecondCalculator()
        {
            HistoricalQuotes = new List<Quote>();
            StdDevResults = new List<StdDevResult>();
        }

        private void CalculateSuggestionPrices()
        {
            CalculateFiveBasicValues();
            MakeSuggestions();

        }

        private void CalculateFiveBasicValues()
        {
            var sixtyEightLowLimit = StdDevResults.Where(t => t.ZScore <= (Decimal)(-1.0) && t.ZScore > (Decimal)(-2.0)).OrderByDescending(t=>t.ZScore).FirstOrDefault();
            var sixtyEightLowLimitPrice = sixtyEightLowLimit.StdDev * sixtyEightLowLimit.ZScore + sixtyEightLowLimit.Mean;

            var sixtyEightHighLimit = StdDevResults.Where(t => t.ZScore <= (Decimal)1.0 && t.ZScore > (Decimal)(0)).OrderByDescending(t=>t.ZScore).FirstOrDefault();
            var sixtyEightHighLimitPrice = sixtyEightHighLimit.StdDev * sixtyEightHighLimit.ZScore + sixtyEightHighLimit.Mean;

            var ninetyLowLimit = StdDevResults.Where(t => t.ZScore > (Decimal)(-2.0) && t.ZScore < (Decimal)(-1.0)).OrderBy(t=> t.ZScore).FirstOrDefault();
            var ninetyLowLimitPrice = ninetyLowLimit.StdDev * ninetyLowLimit.ZScore + ninetyLowLimit.Mean;

            var ninetyHighLimit = StdDevResults.Where(t => t.ZScore <= (Decimal)2.0 && t.ZScore > (Decimal)1.0).OrderByDescending(t=>t.ZScore).FirstOrDefault();
            var ninetyHighLimitPrice = ninetyHighLimit.StdDev * ninetyHighLimit.ZScore + ninetyHighLimit.Mean;

            var mean = StdDevResults.Where(t => t.ZScore < (Decimal)(0) && t.ZScore >(Decimal)(-1)).OrderByDescending(t=>t.ZScore).FirstOrDefault();
            var meanPrice = mean.StdDev * mean.ZScore + mean.Mean;

            var List = new List<decimal> {
                sixtyEightLowLimitPrice.Value,
                sixtyEightHighLimitPrice.Value,
                meanPrice.Value,
                ninetyHighLimitPrice.Value,
                ninetyLowLimitPrice.Value
            };
            var sorted = List.OrderBy(t=>t).ToList();
            FiveBasicNumber = new FiveBasicNumberValue
            {
                NinetyLow = sorted[0],
                SixtyEightLow = sorted[1],
                Mean = sorted[2],
                SixtyEightHigh = sorted[3],
                NinetyHigh = sorted[4]
            };
        }

        private void MakeSuggestions()
        {

            BuyingSuggestion = new Suggestion
            {
                HighLimit = FiveBasicNumber.SixtyEightHigh,
                LowLimit = FiveBasicNumber.SixtyEightLow
            };
            SellingSuggestion = new Suggestion
            {
                HighLimit = FiveBasicNumber.NinetyHigh,
                LowLimit = FiveBasicNumber.NinetyLow
            };
            HoldOrBuySuggestion = new Suggestion
            {
                HighLimit = FiveBasicNumber.SixtyEightLow,
                LowLimit = FiveBasicNumber.NinetyLow
            };
            HoldOrSellSuggestion = new Suggestion
            {
                HighLimit = FiveBasicNumber.NinetyHigh,
                LowLimit = FiveBasicNumber.SixtyEightHigh
            };
        }

        public string GetSuggestions()
        {
            CalculateSuggestionPrices();

            var suggestionList = new List<string>();
            var buyLowLimitPrice = Math.Round(BuyingSuggestion.LowLimit, 2);
            var buyHighLimitPrice = Math.Round(BuyingSuggestion.HighLimit, 2);
            var mean = Math.Round(FiveBasicNumber.Mean, 2);

            suggestionList.Add(SuggestionString(mean, buyHighLimitPrice, SuggestedAction.Buy));
            suggestionList.Add(SuggestionString(buyLowLimitPrice, mean, SuggestedAction.StrongBuy));

            var sellLowLimitPrice = Math.Round(SellingSuggestion.LowLimit, 2);
            var sellHighLimitPrice = Math.Round(SellingSuggestion.HighLimit, 2);
            suggestionList.Add( SuggestionString(sellLowLimitPrice, sellHighLimitPrice, SuggestedAction.Sell));

            var holdToBuyLowLimitPrice = Math.Round(HoldOrBuySuggestion.LowLimit, 2);
            var holdToBuyHighLimitPrice = Math.Round(HoldOrBuySuggestion.HighLimit, 2);
            suggestionList.Add(SuggestionString(holdToBuyLowLimitPrice, holdToBuyHighLimitPrice, SuggestedAction.SellPriceCouldGoDown));

            var holdToSellLowLimitPrice = Math.Round(HoldOrSellSuggestion.LowLimit, 2);
            var holdToSellHighLimitPrice = Math.Round(HoldOrSellSuggestion.HighLimit, 2);
            suggestionList.Add(SuggestionString(holdToSellLowLimitPrice, holdToSellHighLimitPrice, SuggestedAction.HoldPriceCouldGoUp));
            return string.Join(" ", suggestionList);
        }

        private string SuggestionString(decimal lowLimitPrice,
            decimal highLimitPrice, SuggestedAction action)
        {
            if (action == SuggestedAction.Sell)
                return $" StrongSell When x < {lowLimitPrice}. Sell When x > {highLimitPrice}.";

            return $"{action} When {lowLimitPrice} < x < {highLimitPrice}.";

        }

       
    }
}

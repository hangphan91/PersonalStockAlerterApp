using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trady.Analysis.Indicator;

namespace HP.PersonalStocks.Mgr.Factories
{
    public class IndicatorSuggestions
    {
        public bool OSCToBuy { get; set; }
        public bool RSIToBuy { get; set; }
        public bool RSIToSell { get; set; }
        public string GetChaikinOSCValue(List<ChaikinOscResult> osc)
        {
            if (osc == null || osc.Count == 0)
                return "";
            var instruction = $"Buy When OSC Positive. Sell When OSC Negative.";
            var osdValue = osc.LastOrDefault().Oscillator.Value;
            var isPossitive = osdValue > 0;
            OSCToBuy = isPossitive;
            return $"{(isPossitive ? "Positive" : "Negative")} ({instruction})";
        }

        public string GetRSIIndicator(List<StochResult> rsi)
        {
            if (rsi == null || rsi.Count == 0)
                return "";
            var instruction = "Buy When RSI < 30. Sell When RSI > 70.";
            var rsiCurrentValue = Math.Round(rsi.LastOrDefault().Signal.Value, 2);
            var rsiValue = rsiCurrentValue.ToString();
            RSIToBuy = rsiCurrentValue <= 30;
            RSIToSell = rsiCurrentValue >= 70;
            return $"{rsiValue} ({instruction})";
        }
    }
}

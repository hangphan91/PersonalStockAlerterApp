﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HP.PersonalStocksAlerter.Models.Models
{
    public class AlertResult
    {
        public List<string> SuggestedActions { get; set; }
        public string SuggestionMessage { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Symbol { get; set; }
        public string CurrentRSIValue { get; set; }
        public string CurrentChaikinOSCValue { get; set; }
        public string FinalSuggestion { get; set; }
        public decimal CurrentPrice { get; set; }
        public AlertResult()
        {
            SuggestedActions = new List<string>();
            SuggestionMessage = string.Empty;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HP.Data.Definitions.Entities
{
    public class Suggestion
    {
        public long Id { get; set; }
        public string SuggestedActions { get; set; }
        public string FinalSuggestion { get; set; }
        public string SuggestionMessage { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Symbol { get; set; }
        public string CurrentRSIValue { get; set; }
        public string CurrentChaikinOSCValue { get; set; }
        public DateTime InsertTimeStamp { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}

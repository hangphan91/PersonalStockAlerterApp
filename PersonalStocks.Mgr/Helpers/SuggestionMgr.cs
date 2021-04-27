using HP.Data.Definitions.Entities;
using HP.PersonalStocksAlerter.Models.Models;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HP.PersonalStocks.Mgr.Helpers
{
    public static class SuggestionMgr
    {
        public static void SaveSuggestion(AlertResult result)
        {
            using var da = new SuggestionDa();
            da.SaveSuggestion(new Data.Definitions.Entities.Suggestion
            {
                SuggestedActions = string.Join(", ", result.SuggestedActions),
                CurrentChaikinOSCValue = result.CurrentChaikinOSCValue,
                CurrentRSIValue = result.CurrentRSIValue,
                Success = result.Success,
                ErrorMessage = result.ErrorMessage,
                FinalSuggestion = result.FinalSuggestion,
                CurrentPrice = result.CurrentPrice,
                InsertTimeStamp = DateTime.Now,
                SuggestionMessage = result.SuggestionMessage,
                Symbol = result.Symbol
            });
        }
    }
}

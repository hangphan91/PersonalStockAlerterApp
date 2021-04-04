using System;
using System.Text.Json.Serialization;

namespace HP.PersonalStocksAlerter.Models.Models
{
    public class AlertResult
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SuggestedAction SuggestedAction { get; set; }
        public string SuggestionMessage { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public AlertResult()
        {
            SuggestedAction = SuggestedAction.Wait;
            SuggestionMessage = string.Empty;
        }
    }
}

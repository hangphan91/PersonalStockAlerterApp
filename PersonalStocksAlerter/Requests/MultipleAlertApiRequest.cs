using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocksAlerter.Api.Requests
{
    public class MultipleAlertApiRequest
    {
        [Required]
        public List<string> StockStickers { get; set; }
    }
}

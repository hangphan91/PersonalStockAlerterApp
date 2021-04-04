using System;
using System.ComponentModel.DataAnnotations;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocksAlerter.Api.Requests
{
    public class AlertApiRequest
    {
        [Required]
        public StockSticker StockInfo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocksAlerter.Api.Requests
{
    public class AlertApiRequest
    {
        [Required]
        public string StickerSymbol{ get; set; }
    }
}

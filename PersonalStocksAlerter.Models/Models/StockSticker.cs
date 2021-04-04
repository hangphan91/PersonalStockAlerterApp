using System;
using System.ComponentModel.DataAnnotations;

namespace HP.PersonalStocksAlerter.Models.Models
{
    public class StockSticker
    {
        [Required]
        public string StickerSymbol { get; set; }
        public decimal? LowPercentage { get; set; }
        public decimal? HighPercentage { get; set; }
    }
}

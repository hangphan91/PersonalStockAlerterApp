using System;
namespace HP.PersonalStocksAlerter.Models.Models
{
   public enum SuggestedAction
    {
        Wait,
        WaitToBuy,
        WaitToSell,
        Buy,
        Sell,
        HoldPriceCouldGoUp,
        HoldPriceCouldGoDown,
        StrongBuy,
        BuyTheDip,
        RSIToBuy,
        RSIToSell,
        RSIToWait,
        OSCToBuy,
        OSCToSell
    };
    public enum BellShapeValue
    {
        SixtyEight,
        NineTy,
        NinetyFine
    }
}

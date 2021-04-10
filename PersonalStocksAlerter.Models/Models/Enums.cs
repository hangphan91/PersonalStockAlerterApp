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
        SellPriceCouldGoDown,
        StrongBuy,
        StrongSell
    };
    public enum BellShapeValue
    {
        SixtyEight,
        NineTy,
        NinetyFine
    }
}

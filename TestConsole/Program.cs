using System;
using HP.PersonalStocks.Mgr;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new AlertMgr("MSFT");
            
           // var suggest = mgr.GetSuggestionForCurrentSticker();

            //var result = mgr.CheckForAlert();
        }
    }
}

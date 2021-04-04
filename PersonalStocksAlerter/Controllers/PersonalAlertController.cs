using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HP.PersonalStocks.Mgr;
using HP.PersonalStocksAlerter.Api.ActionResults;
using HP.PersonalStocksAlerter.Api.Requests;
using HP.PersonalStocksAlerter.Api.Responses;
using HP.PersonalStocksAlerter.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace HP.PersonalStocksAlerter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalAlertController :ControllerBase
    {
        [HttpPost]
        [Route("GetMultipleAlerts")]
        public AlertMultipleActionResult GetMultipleAlertReult([FromBody] MultipleAlertApiRequest request)
        {
            try
            {
                
                if (!ModelState.IsValid)
                    return new AlertMultipleActionResult
                    {
                        Exception = new Exception("InValid Input Data")
                    };
                var result = new AlertMultipleActionResult();
                var taskFactory = new TaskFactory();
                var tasks = new List<Task<AlertResult>>();

                foreach (var item in request.StockStickers)
                {
                    tasks.Add(Task.Factory.StartNew(()=>
                    {
                        return GetSingleAlert(item);
                    }));
                }
                Task.WaitAll(tasks.ToArray());
                foreach (var task in tasks)
                {
                    result.AlertResults.Add(task.Result);
                }
                return result;
            }
            catch (Exception ex)
            {
                return new AlertMultipleActionResult(ex);
            }
        }
        [HttpPost]
        [Route("GelSingleAlert")]
        public AlertResultActionResult GetAlertResult([FromBody] AlertApiRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new AlertResultActionResult(
                        new AlertApiResonse { Exception = new Exception("InValid Input") });

                AlertResult result = GetSingleAlert(request);
                var response = new AlertApiResonse
                {
                    AlertResult = result,
                    Exception = result.Success ? null : new Exception(result.ErrorMessage)
                };
                return new AlertResultActionResult(response);
            }
            catch (Exception ex)
            {
                return new AlertResultActionResult(new AlertApiResonse
                {
                    Exception = ex,
                    AlertResult = new AlertResult
                    {
                        Success = false,
                        ErrorMessage = "Errors in execute your request, please retry later."
                    }
                });
            }
        }

        private static AlertResult GetSingleAlert(AlertApiRequest request)
        {
            var symbol = request.StockInfo.StickerSymbol.ToUpper();
            var lowPercentage = request.StockInfo.LowPercentage;
            var highPercentage = request.StockInfo.HighPercentage;
            var task = Task.Run(() =>
            {
                var mgr = (lowPercentage.HasValue && highPercentage.HasValue
                && lowPercentage.Value > 0 && highPercentage.Value > 0) ?
                new AlertMgr(symbol, lowPercentage.Value, highPercentage.Value) : new AlertMgr(symbol);
                return mgr;
            });
            task.Wait();
            var myMgr = task.Result;

            var task2 = Task.Run(() =>
            {
                return myMgr.GetAlertResult();
            });
            task2.Wait();
            var result = task2.Result;

            return result;
        }
    }
}

using System;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HP.PersonalStocksAlerter.Api.Responses;
using HP.PersonalStocksAlerter.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;

namespace HP.PersonalStocksAlerter.Api.ActionResults
{
    public class AlertResultActionResult 
    {
        public HttpStatusCode StatusCode { get; set; }
        public AlertResult AlertResult { get; set; }
        public AlertResultActionResult(AlertApiResonse alertResult)
        {
            StatusCode = alertResult.AlertResult.Success ? HttpStatusCode.OK:
                HttpStatusCode.BadRequest;
            AlertResult = alertResult.AlertResult;
        }

       
    }
}

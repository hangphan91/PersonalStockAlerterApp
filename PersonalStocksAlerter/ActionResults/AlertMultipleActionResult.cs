using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocksAlerter.Api.ActionResults
{
    public class AlertMultipleActionResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public Exception Exception { get; set; }
        public List<AlertResult> AlertResults { get; set; }
        public AlertMultipleActionResult(Exception ex)
        {
            StatusCode = HttpStatusCode.BadRequest;
            Exception = ex;
        }
        public AlertMultipleActionResult()
        {
            AlertResults = new List<AlertResult>();
        }
    }
}

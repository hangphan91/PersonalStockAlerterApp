using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using HP.PersonalStocksAlerter.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace HP.PersonalStocksAlerter.Api.Responses
{
    public class AlertApiResonse 
    {
        public Exception Exception { get; set; }
        public AlertResult AlertResult { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticAzure.Telemetry
{
    public enum Services
    {
        PragmaticAzureApplication = 1,
        PragmaticAzureInvoicingService
    }

    // alternative style:
    /*
    public static class Services
    {
        public static readonly string PragmaticAzureApplication = "Pragmatic Azure App";
    }
    */
}

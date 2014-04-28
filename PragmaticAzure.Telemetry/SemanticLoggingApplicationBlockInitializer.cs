using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using EventLevel = System.Diagnostics.Tracing.EventLevel; // TODO: <<== blog about this hack

namespace PragmaticAzure.Telemetry
{
    public class SemanticLoggingApplicationBlockInitializer
    {
        public static EventListener StartListener(string storageConnectionString)
        {
            var now = DateTime.UtcNow;
            var azureTableName = String.Format("PragmaticAzureETWon{0:0000}{1:00}{2:00}at{3:00}{4:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute);
            var azureTableListener = WindowsAzureTableLog.CreateListener(
               "Pronounce", storageConnectionString,
               azureTableName, TimeSpan.FromSeconds(2)); // TODO: be care that production and dev/test settings are appropriate (1s may be good on dev machine, but not prod)

            azureTableListener.EnableEvents(PragmaticAzureEventSource.Logger, EventLevel.LogAlways);

         //   PragmaticAzureEventSource.Logger.AzureTableListenerStarted(azureTableName);

            return azureTableListener;
        }
    }
}




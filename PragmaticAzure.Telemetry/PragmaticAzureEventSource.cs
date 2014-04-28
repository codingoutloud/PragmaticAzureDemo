using System;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace PragmaticAzure.Telemetry
{
    [EventSource(Name = "PragmaticAzure")]
    public class PragmaticAzureEventSource : EventSource
    {
        public static readonly string PragmaticAzureEventSourceName = "PragmaticAzure";

        public static PragmaticAzureEventSource Logger = new PragmaticAzureEventSource();
        // Access from anywhere via: PragmaticAzureEventSource.Logger.SomeMethod(a, b, c)

        internal class EventId
        {
            internal const int AccessAboutPage = 1;
            internal const int AccessContactPage = 2;
        }

        [Event(
            EventId.AccessAboutPage,
            Level = EventLevel.Informational,
            Opcode = EventOpcode.Start,
            Message = "About accessed by {0}")]
        public void AccessAboutPage(string useremail)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(EventId.AccessAboutPage, useremail);
            }
        }


        [NonEvent]
        public void AccessContactPage([CallerMemberName] string methodname = "", [CallerFilePath] string filename = "", [CallerLineNumber] int linenum = -1)
        {
            var useremail = ClaimsPrincipal.Current.Identity.Name;
            var userid = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            var roles = ClaimsPrincipal.Current.FindAll(ClaimTypes.Role);
            // could also show all roles

            AccessContactPage(useremail, userid, methodname, filename, linenum);
        }

        [Event(
            EventId.AccessContactPage, 
            Level = EventLevel.Informational, 
            Opcode = EventOpcode.Start, 
            Message = "Contact accessed by {0} [IdP/App id: {1}] from method {2} in file {3}:{4}")]
        public void AccessContactPage(string useremail, string userid, string method, string filename, int linenum)
        {
            if (this.IsEnabled())
            {
                this.WriteEvent(EventId.AccessContactPage, useremail, userid, method, filename, linenum);
            }
        }

 
        // App Start, App Stop, App Exception, API Start, API Stop, API Exception, ...
    }
}

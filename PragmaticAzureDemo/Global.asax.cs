using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PragmaticAzureDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static EventListener LogEventListener;

        protected void Application_Start()
        {
            var storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=pragaz;AccountKey=CWjAsxps4tmmU2AjuUERyVUJ2Udtw8WI6EE5/BsgscCJ5CUhzLEidSat6U9HZHZONixjtoEB+fwwC+cH13pSZw==";
                // ConfigurationManager.AppSettings["AzureStorageConnectionString.Logging"];
            LogEventListener = PragmaticAzure.Telemetry.SemanticLoggingApplicationBlockInitializer.StartListener(storageConnectionString);


            AreaRegistration.RegisterAllAreas();
            IdentityConfig.ConfigureIdentity();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void WSFederationAuthenticationModule_RedirectingToIdentityProvider(object sender, RedirectingToIdentityProviderEventArgs e)
        {
            if (!String.IsNullOrEmpty(IdentityConfig.Realm))
            {
                e.SignInRequestMessage.Realm = IdentityConfig.Realm;
            }
        }
    }
}

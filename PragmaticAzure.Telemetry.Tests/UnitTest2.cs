using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PragmaticAzure.Telemetry.Tests
{
    [TestClass]
    public class SemanticLoggingValidationTests
    {
        [TestMethod]
        public void Validate()
        {
            EventSourceAnalyzer.InspectAll(PragmaticAzureEventSource.Logger);
        }
    }
}
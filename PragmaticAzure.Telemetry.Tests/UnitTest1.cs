using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PragmaticAzure.Telemetry.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}



#if false


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapsTelemetry;

namespace Telemetry.Tests.Unit
{
   [TestClass]
   public class Telemetry_SanityChecker
   {
      private class CustomEventListener : EventListener, IObservable<EventEntry>
      {
         public IDisposable Subscribe(IObserver<EventEntry> observer)
         {
            Debug.Assert(false); // TODO: Is this ever called? If not, consider removal!

            return null;
         }

         protected override void OnEventWritten(EventWrittenEventArgs eventData)
         {
            var foo = eventData.ToString();
            Console.WriteLine(eventData.ToString());
         }
      }

#if false
      [TestMethod]
      [Ignore] // build machine won't like this
      public void MakeSureEventSourceTestsRunningAsAdmin()
      {
         // => Admin power required to start ETW listening from an external app
         var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
         Assert.IsNotNull(identity); // Administrator should always have access to this
         var principal = new System.Security.Principal.WindowsPrincipal(identity);
         var isAdmin = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
         Assert.IsTrue(isAdmin);
      }
#endif

      [TestMethod]
      public void MakeSureEventSourceLoggerIsNotNull()
      {
         Assert.IsNotNull(CapsEventSource.Logger);
      }

      [TestMethod]
      [Ignore]
      public void MakeSureEventSourceIsTappable()
      {
         int crank = 2;

         var listener2 = WindowsAzureTableLog.CreateListener(CapsEventSource.Logger.Name + crank,
            //@"UseDevelopmentStorage=true"
                                                    @"DefaultEndpointsProtocol=http;AccountName=capstesttelemetry;AccountKey=MHoRYeyVYEJLgjnt5YXBnmYhmlwkgm+Xu6g+UlqONz66c2y1uJ5WduCibxDlyPTQ44e3INadBkdjoj3EZYpY0w=="
                                                    , "SLOB" + crank
                                                    , TimeSpan.FromSeconds(1)
                                                    , true
                                                    , null);

         var tableListener = WindowsAzureTableLog.CreateListener(
            CapsEventSource.CapsEventSourceName
            // CapsEventSource.Logger.Name
            //"Caps"
            //,@"UseDevelopmentStorage=true;"
            ,@"DefaultEndpointsProtocol=http;AccountName=capstesttelemetry;AccountKey=MHoRYeyVYEJLgjnt5YXBnmYhmlwkgm+Xu6g+UlqONz66c2y1uJ5WduCibxDlyPTQ44e3INadBkdjoj3EZYpY0w=="
            //,@"UseDevelopmentStorage=True;"
            //,@"UseDevelopmentStorage=true" // <<= most correct?
  //          ,"SLAB"
//            ,TimeSpan.FromSeconds(1)
            );

         var tempFolder = @"e:\temp\";
         if (!System.IO.Directory.Exists(tempFolder))
         {
             tempFolder = @"c:\dev\temp\";
             Assert.IsTrue(System.IO.Directory.Exists(tempFolder));
         }

         var eventListeners = new List<EventListener>
                                 {
                                    ConsoleLog.CreateListener(),
                                    new CustomEventListener(),
                                    WindowsAzureTableLog.CreateListener(
                                       //CapsEventSource.CapsEventSourceName,
                                       CapsEventSource.Logger.Name,
                                       //@"UseDevelopmentStorage=true;",
                                       //@"DefaultEndpointsProtocol=http;AccountName=capstesttelemetry;AccountKey=MHoRYeyVYEJLgjnt5YXBnmYhmlwkgm+Xu6g+UlqONz66c2y1uJ5WduCibxDlyPTQ44e3INadBkdjoj3EZYpY0w==",
                                       //@"UseDevelopmentStorage=True;",
                                       @"UseDevelopmentStorage=true" // <<= most correct?
//                                       "SLAB",
//                                       TimeSpan.FromSeconds(1)
                                       ),
                                    FlatFileLog.CreateListener(tempFolder + "slab.log"),
                                    tableListener,
                                    listener2
                                 };

         foreach (var eventListener in eventListeners.Where(eventListener => eventListener != null))
         {
            Contract.Assume(eventListener != null);
            eventListener.EnableEvents(CapsEventSource.Logger, EventLevel.LogAlways);
         }

         Contract.Assume(CapsEventSource.Logger != null);
         for (var i = 0; i < 100; i++)
         {
            CapsEventSource.Logger.Test_SimpleIntNoMethodName(i);
            CapsEventSource.Logger.Test_SimpleInt(i);
            Thread.Sleep(TimeSpan.FromSeconds(0.25));
         }

         Thread.Sleep(TimeSpan.FromSeconds(14.25));

       //  tableListener.Dispose();

         foreach (var eventListener in eventListeners.Where(eventListener => eventListener != null))
         {
            Contract.Assume(eventListener != null);
       //     eventListener.Dispose();
            eventListener.DisableEvents(CapsEventSource.Logger);
         }

         Thread.Sleep(TimeSpan.FromSeconds(2.25));         
      }

      [TestMethod]
      public void MakeSureEventSourceNotBorked()
      {
         var analyzer = new EventSourceAnalyzer()
                           {
                              ExcludeEventListenerEmulation = true,
                              ExcludeWriteEventTypeMapping = true,
                              ExcludeWriteEventTypeOrder = true
                           };
         analyzer.Inspect(CapsEventSource.Logger);
         
         // Do 'em both - if this one passes, WindowsAzureEventLog will be happier
         EventSourceAnalyzer.InspectAll(CapsEventSource.Logger);
      }

      [TestMethod]
      public void Looper_LoopsFor10Seconds_NeverFails()
      {
         string str = "hello", str2 = "goodbye";
         int i = 1;
         long l = 1;
         bool b = true;

         var startTime = DateTime.Now;
         var duration = TimeSpan.FromSeconds(10);

         while (startTime + duration > DateTime.Now)
         {
            Thread.Sleep(TimeSpan.FromSeconds(1));

            CapsEventSource.Logger.JustPassingTheTime(str, i, b, str2, l);
            b = !b;
            i++;
            l *= 2;
            str2 = DateTime.Now.ToLongTimeString();
         }
      }

      [TestMethod]
      [Ignore]
      public void Looper_LoopsFor30Seconds_NeverFails()
      {
         string str = "hello", str2 = "goodbye";
         int i = 1;
         long l = 1;
         bool b = true;

         var startTime = DateTime.Now;
         var duration = TimeSpan.FromSeconds(30);

         while (startTime + duration > DateTime.Now)
         {
            Thread.Sleep(TimeSpan.FromSeconds(0.25));

            if (new Random((int)DateTime.Now.Ticks%197).Next(1, 10) < 7)
            {
               CapsEventSource.Logger.JustPassingTheTime(str, i, b, str2, l);
               b = !b;
               i++;
               l *= 2;
               str2 = DateTime.Now.ToLongTimeString();
            }
            else
            {
               CapsEventSource.Logger.Test_SimpleString("hey there");
            }
         }
      }
   }
}


#endif
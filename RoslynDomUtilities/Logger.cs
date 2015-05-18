using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   [EventSource(Name = "RoslynDom.Logger")]
   public class Logger : EventSource
   {
      // Block direct instantiation
      private Logger() { }
      private static Logger _log;
      public static Logger Log
      {
         get
         {
            if (_log == null)
            { _log = new Logger(); }
            return _log;
         }
      }

      [Event(1)]
      public void InvalidXmlDocumentation(
         string xmlDocString, string errorMsg)
      {
         WriteEvent(1, xmlDocString, errorMsg);
      }

   }
}

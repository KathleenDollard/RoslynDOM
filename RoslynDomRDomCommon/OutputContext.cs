using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynDom.Common
{
   public class OutputContext
   {
      public OutputContext(RDomCorporation corporation)
      {
         Corporation = corporation;
         // TODO: Make public annotation output work and pass context on call, then remove this line 
         SkipPublicAnnotationsOnOutput = true;
         Bag = new Dictionary<string, object>();
      }

      public  RDomCorporation Corporation { get; private set; }
      public bool SkipPublicAnnotationsOnOutput { get; set; }

      /// <summary>
      /// In an extreme circumstances (end regions, where trivia dosn't apear on correct element
      /// and empty regions can occur) I need a get-out-of-jail-free card. Please do not use
      /// this except under duress when other design alternatives are exhausted. 
      /// <br/>
      /// </summary>
      public IDictionary<string, object> Bag { get; }
   }
}

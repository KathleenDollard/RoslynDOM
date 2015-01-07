namespace RoslynDom.Common
{
   public class OutputContext
   {
      public OutputContext(RDomCorporation corporation)
      {
         Corporation = corporation;
         // TODO: Make public annotation output work and pass context on call, then remove this line
         SkipPublicAnnotationsOnOutput = true;
      }

      public RDomCorporation Corporation { get; private set; }

      public bool SkipPublicAnnotationsOnOutput { get; set; }
   }
}
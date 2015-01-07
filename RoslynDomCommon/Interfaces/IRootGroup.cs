namespace RoslynDom.Common
{
   public interface IRootGroup : IDom<IRootGroup>
   {
      RDomCollection<IRoot> Roots { get; }

      bool HasSyntaxErrors { get; }
   }
}
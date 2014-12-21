using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System.ComponentModel.DataAnnotations;
namespace RoslynDom
{
   public class RDomUsingDirective : RDomBase<IUsingDirective, ISymbol>, IUsingDirective
   {
      public RDomUsingDirective( string name, string alias = null)
        : base()
      {
         _name = name;
         _alias = alias;
      }

      public RDomUsingDirective(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
      { }

      internal RDomUsingDirective(RDomUsingDirective oldRDom)
          : base(oldRDom)
      {
         _name = oldRDom.Name;
         _alias = oldRDom.Alias;
      }

      private string _name;
      [Required]
      public string Name
      {
         get { return _name; }
         set { SetProperty(ref _name, value); }
      }
      private string _alias;
      public string Alias
      {
         get { return _alias; }
         set { SetProperty(ref _alias, value); }
      }

      public StemMemberKind StemMemberKind
      { get { return StemMemberKind.UsingDirective; } }
   }
}

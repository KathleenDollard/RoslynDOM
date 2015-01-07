using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynDom.CSharp
{
   // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
   public abstract class RDomBaseSyntaxNodeFactory<T, TSyntax> : RDomBaseFactory<T>, IRDomFactory
      where TSyntax : SyntaxNode
      where T : IDom
   {
      public abstract IEnumerable<SyntaxNode> BuildSyntax(IDom item);

      protected RDomBaseSyntaxNodeFactory(RDomCorporation corporation)
      {
         OutputContext = new OutputContext(corporation);
      }

      public OutputContext OutputContext { get; private set; }

      private ICSharpBuildSyntaxWorker _buildSyntaxWorker;

      internal ICSharpBuildSyntaxWorker BuildSyntaxWorker
      {
         get
         {
            if (_buildSyntaxWorker == null) { _buildSyntaxWorker = (ICSharpBuildSyntaxWorker)OutputContext.Corporation.BuildSyntaxWorker; }
            return _buildSyntaxWorker;
         }
      }

      private ICSharpCreateFromWorker _createFromWorker;

      internal ICSharpCreateFromWorker CreateFromWorker
      {
         get
         {
            if (_createFromWorker == null) { _createFromWorker = (ICSharpCreateFromWorker)OutputContext.Corporation.CreateFromWorker; }
            return _createFromWorker;
         }
      }

      public virtual Type[] SupportedSyntaxNodeTypes
      { get { return new Type[] { typeof(TSyntax) }; } }

      public virtual Type[] SpecialExplicitDomTypes
      { get { return new Type[] { }; } }

      public virtual bool CanCreate(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      { return true; }

      public virtual bool CanCreateSpecialExplicit<TSpecial>(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      { return true; }

      /// <summary>
      /// This is the key method for creating new RoslynDom elements. You can create new factories
      /// by overriding the CreateListFrom or CreateItemFrom methods depending on whether you are
      /// creating a list or single element respectively
      /// </summary>
      /// <param name="syntaxNode"></param>
      /// <param name="parent"></param>
      /// <param name="model"></param>
      /// <returns></returns>
      /// <remarks>
      /// Lists are created when multiple items are declared on a line. This allows simple access to
      /// variables and fields in the predominate case with only one variable per line. Currently this is not
      /// recreated on output but the design intent was to have variable groups with arbitrarily defined
      /// IDs and probably a position in the list. These two properties would be added to Field and Declaration
      /// RDom items, and possibly interfaces. This would be sufficient for recreation of lists
      /// <para/>
      /// Use CanCreate and Priority to control how your factory is selected. Highest priority wins and
      /// you can add to the enum values (as in Normal + 1)
      /// </remarks>
      public IEnumerable<IDom> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model, bool skipWhitespace)
      {
         var ret = new List<IDom>();

         var newItems = CreateListFromInterim(syntaxNode, parent, model);
         if (!newItems.Any()) return ret;

         var lookForTrivia = typeof(IStatement).IsAssignableFrom(typeof(T))
            || typeof(ITypeMember).IsAssignableFrom(typeof(T))
            || typeof(IStemMember).IsAssignableFrom(typeof(T));
         if (lookForTrivia)
         {
            var leading = CreateFromWorker.GetDetail(syntaxNode, syntaxNode.GetLeadingTrivia(),
                        newItems.FirstOrDefault().Parent, model, OutputContext);
            var trailingTrivia = syntaxNode.ChildTokens().Last().LeadingTrivia;
            var trailing = CreateFromWorker.GetDetail(syntaxNode, trailingTrivia,
                        newItems.LastOrDefault(), model, OutputContext);
            ret.AddRange(leading.OfType<IDom>());
            if (trailing.Any() && newItems.Any())
            {
               var newLastAsContainer = newItems.Last() as IContainer;
               foreach (var newTrailing in trailing)
               { newLastAsContainer.AddOrMoveMember(newTrailing); }
            }
            ret.AddRange(newItems.OfType<IDom>());
         }
         else
         { ret.AddRange(newItems.OfType<IDom>()); }
         return ret;
      }

      protected virtual IEnumerable<IDom> CreateListFromInterim(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         return CreateListFrom(syntaxNode, parent, model);
      }

      protected virtual IEnumerable<IDom> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         return new IDom[] { CreateItemFrom(syntaxNode, parent, model) };
      }

      protected virtual IDom CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
      {
         // This can't be tested, but making it abstract means we have untestable code in seven derived classes.
         Guardian.Assert.NeitherCreateFromNorListOverridden(this.GetType(), syntaxNode);
         return null;
      }
   }
}
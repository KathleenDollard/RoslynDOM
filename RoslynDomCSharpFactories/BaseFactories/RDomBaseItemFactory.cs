using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
   public abstract class RDomBaseItemFactory<T, TSyntax> : IRDomFactory
       where TSyntax : SyntaxNode
   {
      // until move to C# 6 - I want to support name of as soon as possible
      [ExcludeFromCodeCoverage]
      protected static string nameof<T2>(T2 value) { return ""; }

      public RDomBaseItemFactory(RDomCorporation corporation)
      {
         Corporation = corporation;
      }

      //public abstract string Language { get; }
      public abstract IEnumerable<SyntaxNode> BuildSyntax(IDom item);

      protected RDomCorporation Corporation { get; private set; }

      public virtual Type[] SyntaxNodeTypes
      { get { return new Type[] { typeof(TSyntax) }; } }

      public virtual Type[] ExplicitNodeTypes
      { get { return null; } }

      public virtual Func<SyntaxNode, IDom, SemanticModel, bool> CanCreateDelegate
      { get { return null; } }

      public virtual Type DomType
      {  get { return typeof(T); } }

      private ICSharpBuildSyntaxWorker _buildSyntaxWorker;
      internal ICSharpBuildSyntaxWorker BuildSyntaxWorker
      {
         get
         {
            if (_buildSyntaxWorker == null) { _buildSyntaxWorker = (ICSharpBuildSyntaxWorker)Corporation.Worker.BuildSyntaxWorker; }
            return _buildSyntaxWorker;
         }

      }

      private ICSharpCreateFromWorker _createFromWorker;
      internal ICSharpCreateFromWorker CreateFromWorker
      {
         get
         {
            if (_createFromWorker == null) { _createFromWorker = (ICSharpCreateFromWorker)Corporation.Worker.CreateFromWorker; }
            return _createFromWorker;
         }

      }

      public virtual RDomPriority Priority
      {
         get
         {
            if (this.GetType().IsConstructedGenericType) { return RDomPriority.Fallback; }
            return RDomPriority.Normal;
         }
      }



      public virtual bool CanCreateFrom(SyntaxNode syntaxNode)
      {
         return (syntaxNode is TSyntax);
      }

      public virtual bool CanBuildSyntax(IDom item)
      {
         // TODO: use this to call correct IStem or IType version of class, interface, structure and enum
         return true;
      }

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
         // Whitespace and comments have to appear before new items 
         if (typeof(IStatement).IsAssignableFrom(typeof(T))
            || typeof(ITypeMember).IsAssignableFrom(typeof(T))
            || typeof(IStemMember).IsAssignableFrom(typeof(T)))
         {
            var newItem = newItems.FirstOrDefault();
            if (newItem != null)
            {
               var whiteComment = CreateFromWorker.GetDetail(syntaxNode, newItem, model);
               ret.AddRange(whiteComment.OfType<IDom>());
            }
         }
         ret.AddRange(newItems.OfType<IDom>());
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

using Microsoft.CodeAnalysis;
using RoslynDom.Common;
using System;
using System.Collections.Generic;

namespace RoslynDom.CSharp
{
   // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
   public abstract class RDomBaseSyntaxTriviaFactory<T> : RDomBaseFactory<T>, ITriviaFactory<T>
       where T : IDom
   {
      private RDomCorporation corporation;

      public RDomCorporation Corporation
      {
         get { return corporation; }
         set
         {
            if (corporation != null) throw new InvalidOperationException("Can't reset corporation");
            corporation = value;
         }
      }

      private ICSharpBuildSyntaxWorker _buildSyntaxWorker;

      internal ICSharpBuildSyntaxWorker BuildSyntaxWorker
      {
         get
         {
            if (_buildSyntaxWorker == null) { _buildSyntaxWorker = (ICSharpBuildSyntaxWorker)Corporation.BuildSyntaxWorker; }
            return _buildSyntaxWorker;
         }
      }

      private ICSharpCreateFromWorker _createFromWorker;

      internal ICSharpCreateFromWorker CreateFromWorker
      {
         get
         {
            if (_createFromWorker == null) { _createFromWorker = (ICSharpCreateFromWorker)Corporation.CreateFromWorker; }
            return _createFromWorker;
         }
      }

      //public override Type[] ExplicitNodeTypes
      //{ get { return new Type[] { typeof(T) }; } }

      public virtual IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom item, OutputContext context)
      {
         throw new NotImplementedException();
      }

      public virtual IDom CreateFrom(SyntaxTrivia trivia, IDom parent, OutputContext context)
      {
         throw new NotImplementedException();
      }
   }
}
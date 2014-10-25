using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
   public abstract class RDomBaseSyntaxTriviaFactory<T> : RDomBaseFactory<T>, ITriviaFactory<T>
       where T : IDom
   {

      public override Type[] ExplicitNodeTypes
      { get { return new Type[] { typeof(T) }; } }

      public virtual IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom item, OutputContext context)
      {
         throw new NotImplementedException();
      }

      public virtual IDom CreateFrom(SyntaxTrivia trivia, OutputContext context)
      {
         throw new NotImplementedException();
      }
   }
}

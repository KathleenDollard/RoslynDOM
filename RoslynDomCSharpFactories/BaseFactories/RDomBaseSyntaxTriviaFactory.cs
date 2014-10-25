using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
   public abstract class RDomBaseSyntaxTriviaFactory<T, TSyntax> : RDomBaseFactory<T>, ITriviaFactory
       where TSyntax : SyntaxNode
       where T : IDom
   {
      protected RDomBaseSyntaxTriviaFactory(RDomCorporation corporation) : base(corporation)
      { }

      public IEnumerable<SyntaxTrivia> BuildSyntaxTrivia(IDom item, OutputContext context)
      {
         throw new NotImplementedException();
      }

      public IDom CreateFrom(SyntaxTrivia trivia, OutputContext context)
      {
         throw new NotImplementedException();
      }

      public IDom CreateFrom(string possibleAnnotation, OutputContext context)
      {
         throw new NotImplementedException();
      }
   }
}

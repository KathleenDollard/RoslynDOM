using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
   // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
   public abstract class RDomBaseFactory<T> 
      where T : IDom
   {
      // until move to C# 6 - I want to support name of as soon as possible
      [ExcludeFromCodeCoverage]
      protected static string nameof<T2>(T2 value) { return ""; }

       public virtual Type[] ExplicitNodeTypes
      { get { return null; } }

      public virtual Type DomType
      {  get { return typeof(T); } }

      public virtual RDomPriority Priority
      {
         get
         {
            if (this.GetType().IsConstructedGenericType) { return RDomPriority.Fallback; }
            return RDomPriority.Normal;
         }
      }
   }
}

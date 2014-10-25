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

      protected RDomBaseFactory(RDomCorporation corporation)
      {
         OutputContext = new OutputContext(corporation);
      }

      public OutputContext OutputContext { get; private set; }

      public virtual Type[] ExplicitNodeTypes
      { get { return null; } }

      public virtual Type DomType
      {  get { return typeof(T); } }

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

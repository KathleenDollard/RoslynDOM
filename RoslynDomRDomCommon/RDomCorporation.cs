using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
   // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)

   public class RDomCorporation
   {
      // WARNING: At present you must register all factories before retrieving any. 

      // until move to C# 6 - I want to support name of as soon as possible
      protected static string nameof<T>(T value) { return ""; }

      private Provider provider = new Provider();
      private Dictionary<Type, Tuple<IRDomFactory, IRDomFactory>> domFactoryLookup;
      private Worker worker;
      private bool isInitialized;

      // factory sets are grouped by the types IRoot, IStemMember, ITypeMember, IStatement, IExpression, IMisc and hopefully soon None
      // These types are NOT exclusive, four types are both stem members and type members: class, struct, interface and enum
      private List<Tuple<Type, FactorySet>> factorySets;

      public RDomCorporation()
      {
         provider.ConfigureContainer(this);
         factorySets = new List<Tuple<Type, FactorySet>>();
         var factories = provider.GetFactories();
         var factoryTypeTuple = factories
                           .Select(x => Tuple.Create(x, GetKindType(x.GetType())));
         CreateDomLookup(factoryTypeTuple);
         CreateFactorySets(factoryTypeTuple);

         var createFromWorker = provider.GetCreateFromWorker();
         var buildSyntaxWorker = provider.GetBuildSyntaxWorker();
         worker = new Worker(createFromWorker, buildSyntaxWorker);

      }

      public Worker Worker
      {
         get
         {
            Initialize();
            return worker;
         }
      }

      public IEnumerable<TKind> CreateFrom<TKind>(SyntaxNode node, IDom parent, SemanticModel model)
            where TKind : class
      {
         return CreateFrom<TKind>(node, parent, model, false);
      }

      /// <summary>
      /// 
      /// </summary>
      /// <typeparam name="TKind">
      /// This is the Kind as it will be placed in a container, NOT the specific type. 
      /// For example IStemMemberCommentWhite.
      /// <para/>
      /// This distinction is not important for most classes, but is critical for the four
      /// classes that can be either stem or type members (class, struct, interface, enum),
      /// so it's best to be in the habit of requesting the correct thing. 
      /// </typeparam>
      /// <param name="node"></param>
      /// <param name="parent"></param>
      /// <param name="model"></param>
      /// <returns></returns>
      public IEnumerable<TKind> CreateFrom<TKind>(SyntaxNode node, IDom parent, SemanticModel model, bool skipCommentWhitespace)
          where TKind : class
      {
         Initialize();

         var candidates = GetCandidateFactories(typeof(TKind));
         foreach (var candidate in candidates)
         {
            if (candidate.CanCreateFrom(node))
            {
               var items = candidate.CreateFrom(node, parent, model, skipCommentWhitespace);
               return items.Cast<TKind>()
                           .ToList();
            }
         }
         return Worker.CreateFromWorker.CreateInvalidMembers<TKind>(node, parent, model);
      }

      private List<IRDomFactory> GetCandidateFactories(Type type)
      {
         if (typeof(IAttribute).IsAssignableFrom(type)) { type = typeof(IAttribute); }
         if (typeof(IStructuredDocumentation).IsAssignableFrom(type)) { type = typeof(IStructuredDocumentation); }
         if (typeof(IPublicAnnotation).IsAssignableFrom(type)) { type = typeof(IPublicAnnotation); }
         if (typeof(ICommentWhite).IsAssignableFrom(type)) { type = typeof(ICommentWhite); }

         var candidates = new List<IRDomFactory>();
         // I think you always want to grab an explicit one if it exists
         Tuple<IRDomFactory, IRDomFactory> factoryTuple = null;
         if (domFactoryLookup.TryGetValue(type, out factoryTuple))
         {
            candidates.Add(factoryTuple.Item1);
            if (factoryTuple.Item2 != null)
            { candidates.Add(factoryTuple.Item2); }
         }
         else
         {
            candidates = GetFactoriesOfKind(type)
                              .OrderByDescending(x => x.Priority)
                              .ToList();
         }

         return candidates;
      }

      public IEnumerable<SyntaxNode> BuildSyntaxGroup(IDom item)
      {
         Initialize();
         if (item == null) return new List<SyntaxNode>();
         var candidates = GetCandidateFactories(item.GetType());
         foreach (var factory in candidates)
         {
            if (factory.CanBuildSyntax(item))
            {
               var ret = factory.BuildSyntax(item);
               return ret;
            }
         }
         Guardian.Assert.FactoryNotFound(item);
         return null;
      }

      public int CountFactorySet(Type type)
      {
         var factories = GetFactoriesOfKind(type);
         return factories.Count();
      }

      public bool HasExpectedItems()
      {
         // TODO: Check for workers, ets.
         return true;
      }

      private void Initialize()
      {
         if (isInitialized )
         {
            provider.CheckContainer();
         }
         //if (factorySets == null)
         //{
            //provider.ConfigureContainer(this);
            //factorySets = new List<Tuple<Type, FactorySet>>();
            //var factories = provider.GetFactories();
            //var factoryTypeTuple = factories
            //                  .Select(x => Tuple.Create(x, GetKindType(x.GetType())));
            //CreateDomLookup(factoryTypeTuple);
            //CreateFactorySets(factoryTypeTuple);

            //var createFromWorker = provider.GetCreateFromWorker();
            //var buildSyntaxWorker = provider.GetBuildSyntaxWorker();
            //worker = new Worker(createFromWorker, buildSyntaxWorker);
            //provider.CheckContainer();
         //}
      }

      private void CreateDomLookup(IEnumerable<Tuple<IRDomFactory, Type[]>> factoryTuples)
      {
         domFactoryLookup = new Dictionary<Type, Tuple<IRDomFactory, IRDomFactory>>();
         foreach (var pair in factoryTuples)
         {
            var type = pair.Item2[0];
            if (domFactoryLookup.ContainsKey(type))
            {
               var existing = domFactoryLookup[type];
               domFactoryLookup[type] = Tuple.Create(existing.Item1, pair.Item1);
            }
            else
            { domFactoryLookup.Add(type, new Tuple<IRDomFactory, IRDomFactory>(pair.Item1, null)); }
         }
      }

      private void CreateFactorySets(IEnumerable<Tuple<IRDomFactory, Type[]>> factoryTuples)
      {
         var typeKinds = factoryTuples.Select(x => x.Item2[2]).Distinct();
         foreach (var typeKind in typeKinds)
         {
            var candidates = factoryTuples
                            .Where(x => x.Item2[2] == typeKind)
                            .Select(x => x.Item1);
            var newSet = new FactorySet(typeKind, candidates, null);
            factorySets.Add(Tuple.Create(typeKind, newSet));
         }
      }

      private Type[] GetKindType(Type type)
      {
         while (type != null && type.GenericTypeArguments.Count() < 3)
         { type = type.BaseType; }
         Guardian.Assert.IsNotNull(type, nameof(type));
         return type.GenericTypeArguments;
      }

      private IEnumerable<IRDomFactory> GetFactoriesOfKind(Type kind)
      {
         Tuple<IRDomFactory, IRDomFactory> factory = null;
         if (domFactoryLookup.TryGetValue(kind, out factory))
         {
            var list = new List<IRDomFactory>() { factory.Item1 };
            if (factory.Item2 != null) { list.Add(factory.Item2); }
            return list;
         }

         var factorySet = GetFactorySet(kind);
         Guardian.Assert.FactorySetExists(factorySet, kind, nameof(factorySet));
         return factorySet.Factories;
      }

      private FactorySet GetFactorySet(Type kind)
      {
         foreach (var setPair in factorySets)
         {
            // TODO: Perhaps this should be IsAssignableFrom
            if (setPair.Item1 == kind)
            { return setPair.Item2; }
         }
         return null;
      }

   }

   internal class FactorySet
   {
      private IEnumerable<IRDomFactory> factories;
      private IRDomFactory genericFactory;
      private Type typeKind;

      internal FactorySet(Type typeKind, IEnumerable<IRDomFactory> factories, IRDomFactory genericFactory)
      {
         this.factories = factories;
         this.typeKind = typeKind;
         this.genericFactory = genericFactory;
      }


      internal IEnumerable<IRDomFactory> Factories
      { get { return factories; } }

   }
}

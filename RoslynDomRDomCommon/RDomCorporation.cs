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

      private Provider provider2;
      private Provider provider = new Provider();
      private Worker worker;

      // factory sets are grouped by the types IRoot, IStemMember, ITypeMember, IStatement, IExpression, IMisc and hopefully soon None
      // These types are NOT exclusive, four types are both stem members and type members: class, struct, interface and enum
      private List<Tuple<Type, FactorySet>> factorySets;

      public RDomCorporation()
      { }

      public Worker Worker
      {
         get
         {
            //Initialize();
            return worker;
         }
      }

      #region Spike code

      private IDictionary<Type, IRDomFactory> domTypeLookup = new Dictionary<Type, IRDomFactory>();
      private IDictionary<Type, IRDomFactory> explicitNodeLookup = new Dictionary<Type, IRDomFactory>();
      private IDictionary<Type, IRDomFactory> syntaxNodeLookup = new Dictionary<Type, IRDomFactory>();
      private List<Tuple<Func<SyntaxNode, IDom, SemanticModel, bool>, IRDomFactory>> canCreateList =
                  new List<Tuple<Func<SyntaxNode, IDom, SemanticModel, bool>, IRDomFactory>>();

      public RDomCorporation(string language)
      {
         provider2 = new Provider();
         provider2.ConfigureContainer(this);
         LoadFactories(language.Replace("C#", "CSharp"));
         var createFromWorker = provider2.GetCreateFromWorker();
         var buildSyntaxWorker = provider2.GetBuildSyntaxWorker();
         worker = new Worker(createFromWorker, buildSyntaxWorker);
      }

      public bool CheckContainer()
      { return provider.CheckContainer(); }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="language"></param>
      /// <remarks>
      /// Factories can have explicit and non-explicit usage, non-explicit is the general case but
      /// some special purpose things including comments/regions, public annotations, structured
      /// documentatio, and expressions have explicit calls.
      /// <para/>
      /// If a factory has a create delegate, any syntax node lookup will be ignored. This is to 
      /// allow simpler use of a default
      /// <para/>
      /// The SyntaxNode comparison is absolute against the type. Fuzzy comparisons (including derived
      /// classes, should be done via the delegate.
      /// </remarks>
      private void LoadFactories(string language)
      {
         // The following condition requires that the last part of the namespace of factories state their language
         var factories = provider2
                        .GetFactories()
                        .Where(x => x.GetType().Namespace.SubstringAfterLast(".").Equals(language, StringComparison.InvariantCultureIgnoreCase));
         foreach (var factory in factories)
         {
            bool inUse = false;
            if (domTypeLookup.Keys.Contains(factory.DomType)) Console.WriteLine();
            domTypeLookup.Add(factory.DomType, factory);
            if (AddFactoriesToLookup(factory, factory.ExplicitNodeTypes, explicitNodeLookup)) { inUse = true; }
            if (factory.CanCreateDelegate != null)
            {
               canCreateList.Add(Tuple.Create(factory.CanCreateDelegate, factory));
               inUse = true;
            }
            else if (AddFactoriesToLookup(factory, factory.SyntaxNodeTypes, syntaxNodeLookup)) { inUse = true; }
            if (!inUse)
            { Guardian.Assert.UnreachableFactoryDetected(factory.GetType().FullName); }
         }
      }

      private bool AddFactoriesToLookup(IRDomFactory factory, Type[] types, IDictionary<Type, IRDomFactory> dictionary)
      {
         if (types == null) { return false; }
         foreach (var type in types)
         {
            if (dictionary.Keys.Contains(type))
            { Guardian.Assert.DuplicateFactories(type.Name, factory.GetType().FullName); }
            else
            { dictionary.Add(type, factory); }
         }
         return true;
      }

      public IEnumerable<IDom> Create(SyntaxNode node, IDom parent, SemanticModel model, bool skipCommentWhitespace = false)
      {
         return FindFactoryAndCreate(node.GetType(), syntaxNodeLookup, node, parent, model);
      }

      public IEnumerable<T> Create<T>(SyntaxNode node, IDom parent, SemanticModel model)
         where T : IDom
      {
         return FindFactoryAndCreate(typeof(T), explicitNodeLookup, node, parent, model)
                  .OfType<T>();
      }

      private IEnumerable<IDom> FindFactoryAndCreate(Type type,
               IDictionary<Type, IRDomFactory> dictionary,
               SyntaxNode node, IDom parent, SemanticModel model)
      {
         var factory = GetFactory(type, dictionary, node, parent, model);
         if (factory == null)
         {
            return Worker.CreateFromWorker.CreateInvalidMembers2(node, parent, model);
         }
         var items = factory.CreateFrom(node, parent, model, false);
         return items.ToList();
      }


      public IEnumerable<SyntaxNode> GetSyntaxNodes(IDom item)
      {
         if (item == null) return new List<SyntaxNode>();
         IRDomFactory factory;
         if (domTypeLookup.TryGetValue(item.GetType(), out factory))
         {
            var ret = factory.BuildSyntax(item);
            return ret;
         }
         return null;
      }

      private IRDomFactory GetFactory(Type type, IDictionary<Type, IRDomFactory> dictionary, SyntaxNode node, IDom parent, SemanticModel model)
      {
         IRDomFactory factory;
         if (!(dictionary.TryGetValue(type, out factory)))
         {
            foreach (var tuple in canCreateList)
            {
               if (tuple.Item1(node, parent, model))
               { factory = tuple.Item2; }
            }
         }

         if (factory == null)
         { Guardian.Assert.FactoryNotFound(node); }

         return factory;
      }

      #endregion



      public bool HasExpectedItems()
      {
         // TODO: Check for workers, ets.
         return true;
      }

      #region Code I'm working to replace
      //public int CountFactorySet(Type type)
      //{
      //   var factories = GetFactoriesOfKind(type);
      //   return factories.Count();
      //} 

      //private Dictionary<Type, Tuple<IRDomFactory, IRDomFactory>> domFactoryLookup;

      //public IEnumerable<TKind> CreateFrom<TKind>(SyntaxNode node, IDom parent, SemanticModel model)
      //      where TKind : class
      //{
      //   return CreateFrom<TKind>(node, parent, model, false);
      //}

      ///// <summary>
      ///// 
      ///// </summary>
      ///// <typeparam name="TKind">
      ///// This is the Kind as it will be placed in a container, NOT the specific type. 
      ///// For example IStemMemberCommentWhite.
      ///// <para/>
      ///// This distinction is not important for most classes, but is critical for the four
      ///// classes that can be either stem or type members (class, struct, interface, enum),
      ///// so it's best to be in the habit of requesting the correct thing. 
      ///// </typeparam>
      ///// <param name="node"></param>
      ///// <param name="parent"></param>
      ///// <param name="model"></param>
      ///// <returns></returns>
      //public IEnumerable<TKind> CreateFrom<TKind>(SyntaxNode node, IDom parent, SemanticModel model, bool skipCommentWhitespace)
      //    where TKind : class
      //{
      //   Initialize();

      //   var candidates = GetCandidateFactories(typeof(TKind));
      //   foreach (var candidate in candidates)
      //   {
      //      if (candidate.CanCreateFrom(node))
      //      {
      //         var items = candidate.CreateFrom(node, parent, model, skipCommentWhitespace);
      //         return items.Cast<TKind>()
      //                     .ToList();
      //      }
      //   }
      //   return Worker.CreateFromWorker.CreateInvalidMembers<TKind>(node, parent, model);
      //}

      //private List<IRDomFactory> GetCandidateFactories(Type type)
      //{
      //   if (typeof(IAttribute).IsAssignableFrom(type)) { type = typeof(IAttribute); }
      //   if (typeof(IStructuredDocumentation).IsAssignableFrom(type)) { type = typeof(IStructuredDocumentation); }
      //   if (typeof(IPublicAnnotation).IsAssignableFrom(type)) { type = typeof(IPublicAnnotation); }
      //   if (typeof(ICommentWhite).IsAssignableFrom(type)) { type = typeof(ICommentWhite); }

      //   var candidates = new List<IRDomFactory>();
      //   // I think you always want to grab an explicit one if it exists
      //   Tuple<IRDomFactory, IRDomFactory> factoryTuple = null;
      //   if (domFactoryLookup.TryGetValue(type, out factoryTuple))
      //   {
      //      candidates.Add(factoryTuple.Item1);
      //      if (factoryTuple.Item2 != null)
      //      { candidates.Add(factoryTuple.Item2); }
      //   }
      //   else
      //   {
      //      candidates = GetFactoriesOfKind(type)
      //                        .OrderByDescending(x => x.Priority)
      //                        .ToList();
      //   }

      //   return candidates;
      //}

      //public IEnumerable<SyntaxNode> BuildSyntaxGroup(IDom item)
      //{
      //   return GetSyntaxNodes(item);
         //Initialize();
         //if (item == null) return new List<SyntaxNode>();
         //var candidates = GetCandidateFactories(item.GetType());
         //foreach (var factory in candidates)
         //{
         //   if (factory.CanBuildSyntax(item))
         //   {
         //      var ret = factory.BuildSyntax(item);
         //      return ret;
         //   }
         //}
         //Guardian.Assert.FactoryNotFound(item);
         //return null;
      //}

      //private void Initialize()
      //{
         //if (factorySets == null)
         //{
         //   provider.ConfigureContainer(this);
         //   factorySets = new List<Tuple<Type, FactorySet>>();
         //   var factories = provider.GetFactories();
         //   var factoryTypeTuple = factories
         //                     .Select(x => Tuple.Create(x, GetKindType(x.GetType())));
         //   CreateDomLookup(factoryTypeTuple);
         //   CreateFactorySets(factoryTypeTuple);

         //   var createFromWorker = provider.GetCreateFromWorker();
         //   var buildSyntaxWorker = provider.GetBuildSyntaxWorker();
         //   worker = new Worker(createFromWorker, buildSyntaxWorker);
         //   provider.CheckContainer();
         //}
      //}

      //private void CreateDomLookup(IEnumerable<Tuple<IRDomFactory, Type[]>> factoryTuples)
      //{
      //   domFactoryLookup = new Dictionary<Type, Tuple<IRDomFactory, IRDomFactory>>();
      //   foreach (var pair in factoryTuples)
      //   {
      //      var type = pair.Item2[0];
      //      if (domFactoryLookup.ContainsKey(type))
      //      {
      //         var existing = domFactoryLookup[type];
      //         domFactoryLookup[type] = Tuple.Create(existing.Item1, pair.Item1);
      //      }
      //      else
      //      { domFactoryLookup.Add(type, new Tuple<IRDomFactory, IRDomFactory>(pair.Item1, null)); }
      //   }
      //}

      //private void CreateFactorySets(IEnumerable<Tuple<IRDomFactory, Type[]>> factoryTuples)
      //{
      //   var typeKinds = factoryTuples.Select(x => x.Item2[2]).Distinct();
      //   foreach (var typeKind in typeKinds)
      //   {
      //      var candidates = factoryTuples
      //                      .Where(x => x.Item2[2] == typeKind)
      //                      .Select(x => x.Item1);
      //      var newSet = new FactorySet(typeKind, candidates, null);
      //      factorySets.Add(Tuple.Create(typeKind, newSet));
      //   }
      //}

      //private Type[] GetKindType(Type type)
      //{
      //   while (type != null && type.GenericTypeArguments.Count() < 3)
      //   { type = type.BaseType; }
      //   Guardian.Assert.IsNotNull(type, nameof(type));
      //   return type.GenericTypeArguments;
      //}

      //private IEnumerable<IRDomFactory> GetFactoriesOfKind(Type kind)
      //{
      //   Tuple<IRDomFactory, IRDomFactory> factory = null;
      //   if (domFactoryLookup.TryGetValue(kind, out factory))
      //   {
      //      var list = new List<IRDomFactory>() { factory.Item1 };
      //      if (factory.Item2 != null) { list.Add(factory.Item2); }
      //      return list;
      //   }

      //   var factorySet = GetFactorySet(kind);
      //   Guardian.Assert.FactorySetExists(factorySet, kind, nameof(factorySet));
      //   return factorySet.Factories;
      //}

      //private FactorySet GetFactorySet(Type kind)
      //{
      //   foreach (var setPair in factorySets)
      //   {
      //      // TODO: Perhaps this should be IsAssignableFrom
      //      if (setPair.Item1 == kind)
      //      { return setPair.Item2; }
      //   }
      //   return null;
      //}

      #endregion

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

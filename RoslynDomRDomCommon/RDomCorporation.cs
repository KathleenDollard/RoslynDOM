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
      // until move to C# 6 - I want to support name of as soon as possible
      protected static string nameof<T>(T value) { return ""; }

      private Provider provider2;
      private List<IWorker> workers = new List<IWorker>();
      private ICreateFromWorker createFromWorker;
      private IBuildSyntaxWorker buildSyntaxWorker;
      private IRDomCompilationFactory compilationFactory;
      private IDictionary<Type, IList<IRDomFactory>> domTypeLookup = new Dictionary<Type, IList<IRDomFactory>>();
      private IDictionary<Type, IList<IRDomFactory>> syntaxNodeLookup = new Dictionary<Type, IList<IRDomFactory>>();
      private IDictionary<Type, IList<IRDomFactory>> specialCreateLookup = new Dictionary<Type, IList<IRDomFactory>>();
      //private List<Tuple<Func<SyntaxNode, IDom, SemanticModel, bool>, IRDomFactory>> canCreateList =
      //            new List<Tuple<Func<SyntaxNode, IDom, SemanticModel, bool>, IRDomFactory>>();
      private IFactoryAccess factoryAccess;

      public RDomCorporation(string language, IFactoryAccess factoryAccess)
      {
         language = language.Replace(ExpectedLanguages.CSharp, ExpectedLanguages.CSharpInSymbols);
         this.factoryAccess = factoryAccess;
         provider2 = new Provider();
         provider2.ConfigureContainer(this);
         LoadFactories(language);
         LoadWorkers(language);
      }

      private void LoadWorkers(string language)
      {
         workers = provider2
                        .GetItems<IWorker>()
                        .Where(x =>
                        {
                           var lang = x.GetType().Namespace.SubstringAfterLast(".");
                           return lang == null || lang.Equals(language, StringComparison.InvariantCultureIgnoreCase);
                        })
                        .ToList();
         foreach (var worker in workers.OfType<ICorporationWorker>())
         { worker.Corporation = this; }

         compilationFactory = GetCompilationFactory();
      }

      private IRDomCompilationFactory GetCompilationFactory()
      {
         return provider2.GetItems<IRDomCompilationFactory>()
                                      .OrderByDescending(x => x.Priority)
                                      .First();
      }

      public IFactoryAccess FactoryAccess
      { get { return factoryAccess; } }

      public ICreateFromWorker CreateFromWorker
      {
         get
         {
            if (createFromWorker == null)
            { createFromWorker = workers.OfType<ICreateFromWorker>().FirstOrDefault(); }
            return createFromWorker;
         }
      }

      public IBuildSyntaxWorker BuildSyntaxWorker
      {
         get
         {
            if (buildSyntaxWorker == null)
            { buildSyntaxWorker = workers.OfType<IBuildSyntaxWorker>().FirstOrDefault(); }
            return buildSyntaxWorker;
         }
      }

      public ITriviaFactory<T> GetTriviaFactory<T>()
      {
         return workers.OfType<ITriviaFactory<T>>().SingleOrDefault();
      }

      public T GetWorker<T>()
      {
         return workers.OfType<T>().SingleOrDefault();
      }

      public bool CheckContainer()
      { return provider2.CheckContainer(); }

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
                        .GetItems<IRDomFactory>()
                        .Where(x => x.GetType().Namespace.SubstringAfterLast(".").Equals(language, StringComparison.InvariantCultureIgnoreCase));
         foreach (var factory in factories)
         {
            AddFactoryToLookup(domTypeLookup, factory, factory.SupportedDomTypes);
            AddFactoryToLookup(syntaxNodeLookup, factory, factory.SupportedSyntaxNodeTypes);
            AddFactoryToLookup(specialCreateLookup, factory, factory.SpecialExplicitDomTypes);
         }
      }


      private void AddFactoryToLookup(IDictionary<Type, IList<IRDomFactory>> dictionary, IRDomFactory factory, Type[] types)
      {
         foreach (var type in types)
         {
            IList<IRDomFactory> list;
            if (dictionary.TryGetValue(type, out list))
            {
               // Insert descending
               var next = list.FirstOrDefault(x => x.Priority < factory.Priority);
               if (next == null)
               { list.Add(factory); }
               else
               { list.Insert(list.IndexOf(next), factory); }
               return;
            }
            dictionary.Add(type, new List<IRDomFactory>(new[] { factory }));
         }
      }

      public IEnumerable<IDom> Create(SyntaxNode node, IDom parent, SemanticModel model, bool skipDetail = false)
      {
         var factory = GetFactory(node.GetType(), syntaxNodeLookup, x => x.CanCreate(node, parent, model));
         if (factory == null)
         {
            return createFromWorker.CreateInvalidMembers(node, parent, model);
         }
         var items = factory.CreateFrom(node, parent, model, skipDetail);
         return items.ToList();
      }

      public IEnumerable<TSpecial> CreateSpecial<TSpecial>(SyntaxNode node, IDom parent, SemanticModel model, bool skipDetail = false)
      {
         var factory = GetFactory(typeof(TSpecial), specialCreateLookup, x => x.CanCreateSpecialExplicit<TSpecial>(node, parent, model));
         if (factory == null)
         {
            throw new InvalidOperationException();
         }
         var items = factory.CreateFrom(node, parent, model, skipDetail);
         return items.Cast<TSpecial>().ToList();
      }

       private IRDomFactory GetFactory(Type type, IDictionary<Type, IList<IRDomFactory>> dictionary, Func<IRDomFactory, bool> canUseDelegate)
      {
         var factories = GetFactories(type, dictionary).ToArray();
         if (!factories.Any()) { return null; }
         // factories are already ordered by priority
         var count = factories.Count();
         for (int i = 0; i < count; i++)
         {
            var candidate = factories[i];
            if (canUseDelegate(candidate))
            {
               var indexCheck = i + 1;
               var isOK = true;
               while (indexCheck < count
                     && factories[indexCheck].Priority == candidate.Priority)
               {
                  if (canUseDelegate(factories[indexCheck]))
                  {
                     isOK = false;
                     throw new InvalidOperationException(); // Change to gaurdian, isOK is for that scenario
                  }
               }
               if (isOK)
               { return candidate; }
            }
         }
         return null;
      }

      private IEnumerable<IRDomFactory> GetFactories(Type type, IDictionary<Type, IList<IRDomFactory>> dictionary)
      {
         IList<IRDomFactory> factories;
         if (dictionary.TryGetValue(type, out factories)) { return factories; }
         var baseType = type.BaseType;
         if (baseType == null || baseType == typeof(object)) { return new List<IRDomFactory>(); }
         return GetFactories(baseType, dictionary);
      }

      public IRootGroup CreateCompilation(Compilation compilation, IDom parent, SemanticModel model, bool skipDetail = false)
      {
         return compilationFactory.CreateFrom(compilation, skipDetail);
      }

      //public IEnumerable<T> Create<T>(SyntaxNode node, IDom parent, SemanticModel model)
      //   where T : IDom
      //{
      //   return FindFactoryAndCreate(typeof(T), explicitNodeLookup, node, parent, model)
      //            .OfType<T>();
      //}

      //private IEnumerable<IDom> FindFactoryAndCreate(Type type,
      //         IDictionary<Type, IRDomFactory> dictionary,
      //         SyntaxNode node, IDom parent, SemanticModel model)
      //{
      //   var factory = GetFactory(type, dictionary, node, parent, model);
      //   if (factory == null)
      //   {
      //      return createFromWorker.CreateInvalidMembers(node, parent, model);
      //   }
      //   var items = factory.CreateFrom(node, parent, model, false);
      //   return items.ToList();
      //}

      public IEnumerable<SyntaxNode> GetSyntaxNodes(IDom item)
      {
         if (item == null) return new List<SyntaxNode>();
         var factory = GetFactory(item.GetType(), domTypeLookup, x => x.CanGetSyntax(item));
         if (factory == null) { return new List<SyntaxNode>(); }
         var nodes = factory.BuildSyntax(item);
         return nodes;
      }

      //private IRDomFactory GetFactory(Type type, IDictionary<Type, IRDomFactory> dictionary, SyntaxNode node, IDom parent, SemanticModel model)
      //{
      //   IRDomFactory factory;
      //   if (!(dictionary.TryGetValue(type, out factory)))
      //   {
      //      foreach (var tuple in canCreateList)
      //      {
      //         if (tuple.Item1(node, parent, model))
      //         { factory = tuple.Item2; }
      //      }
      //   }

      //   if (factory == null)
      //   { Guardian.Assert.FactoryNotFound(node); }

      //   return factory;
      //}
   }
}

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
      // private Worker worker;

      public RDomCorporation(string language)
      {
         language = language.Replace("C#", "CSharp");
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
      }

      public ICreateFromWorker CreateFromWorker
      {
         get
         {
            if (createFromWorker == null)
            { createFromWorker = workers.OfType<ICreateFromWorker>().FirstOrDefault(); }
            return createFromWorker;
         }
      }

      public IBuildSyntaxWorker  BuildSyntaxWorker
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
         return workers.OfType < ITriviaFactory<T>>().SingleOrDefault();
      }

      public T GetWorker<T>()
      {
         return workers.OfType<T>().SingleOrDefault();
      }

      private IDictionary<Type, IRDomFactory> domTypeLookup = new Dictionary<Type, IRDomFactory>();
      private IDictionary<Type, IRDomFactory> explicitNodeLookup = new Dictionary<Type, IRDomFactory>();
      private IDictionary<Type, IRDomFactory> syntaxNodeLookup = new Dictionary<Type, IRDomFactory>();
      private List<Tuple<Func<SyntaxNode, IDom, SemanticModel, bool>, IRDomFactory>> canCreateList =
                  new List<Tuple<Func<SyntaxNode, IDom, SemanticModel, bool>, IRDomFactory>>();

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

      public IEnumerable<IDom> Create(SyntaxNode node, IDom parent, SemanticModel model, bool skipDetail = false)
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
            return createFromWorker.CreateInvalidMembers(node, parent, model);
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
   }
}

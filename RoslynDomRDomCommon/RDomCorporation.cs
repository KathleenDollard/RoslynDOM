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

        private Provider provider = new Provider();
        private Dictionary<Type, Tuple<IRDomFactory, IRDomFactory>> domFactoryLookup;
        private Worker worker;

        // factory sets are grouped by the types IRoot, IStemMember, ITypeMember, IStatement, IExpression, IMisc and hopefully soon None
        // These types are NOT exclusive, four types are both stem members and type members: class, struct, interface and enum
        private List<Tuple<Type, FactorySet>> factorySets;


        public Worker Worker
        {
            get
            {
                Initialize();
                return worker;
            }
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
        public IEnumerable<TKind> CreateFrom<TKind>(SyntaxNode node, IDom parent, SemanticModel model)
            where TKind : class
        {
            Initialize();

            var candidates = GetCandidateFactories(typeof(TKind));
            foreach (var candidate in candidates)
            {
                if (candidate.CanCreateFrom(node))
                {
                    var items = candidate.CreateFrom(node, parent, model);
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
            if (item == null) return null;
            var candidates = GetCandidateFactories(item.GetType());
            foreach (var factory in candidates)
            {
                if (factory.CanBuildSyntax(item))
                {
                    var ret = factory.BuildSyntax(item);
                    return ret;
                }
            }
            throw new InvalidOperationException();
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
            if (factorySets == null)
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
                provider.CheckContainer();
            }
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
            if (type == null) { throw new NotImplementedException(); }
            return type.GenericTypeArguments;
        }

        private IEnumerable<IRDomFactory> GetFactoriesOfKind(Type kind)
        {
            var factorySet = GetFactorySet(kind);
            if (factorySet != null) { return factorySet.Factories; }
            Tuple<IRDomFactory, IRDomFactory> factory = null;
            if (domFactoryLookup.TryGetValue(kind, out factory))
            {
                var list = new List<IRDomFactory>() { factory.Item1 };
                if (factory.Item2 != null) { list.Add(factory.Item2); }
                return list;
            }
            return null;
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

        //private IEnumerable<IRDomFactory> GetCandidates(Type type, SyntaxNode node)
        //{
        //    var candidates = GetFactoriesOfKind(type);
        //    var found = candidates
        //                      .Where(x => x.GetType().IsAssignableFrom(type))
        //                      .Select(x => x);
        //    return found;
        //}

        //private List<Type> RDomKinds = new List<Type>()
        //        {
        //            typeof(IRoot),
        //            typeof(IStemMember),
        //            typeof(ITypeMember),
        //            typeof(IStatement),
        //            typeof(IExpression),
        //            typeof(IMisc)
        //       };

        //public  void Register<TKind>(RDomFactoryHelper factoryHelper)
        //    where TKind : class, IDom
        //{
        //    registration.Add(Tuple.Create(typeof(TKind), factoryHelper));
        //}

        ////private static RDomFactoryHelper<TKind> GetHelper<TKind>()
        ////    where TKind : class, IDom
        ////{
        ////    foreach (var tuple in registration)
        ////    {
        ////        if (tuple.Item1 == typeof(TKind)) { return (RDomFactoryHelper<TKind>)tuple.Item2; }
        ////    }
        ////    throw new InvalidOperationException();
        ////}

        ////// Added these because of bugs with random TKind - wrong or without CommentWhite - causing missing factories - which is hard to recover from
        ////public static RDomFactoryHelper<IRoot> GetHelperForRoot()
        ////{ return GetHelper<IRoot>(); }
        ////public static RDomFactoryHelper<IStemMemberCommentWhite> GetHelperForStemMember()
        ////{ return GetHelper<IStemMemberCommentWhite>(); }
        ////public static RDomFactoryHelper<ITypeMemberCommentWhite> GetHelperForTypeMember()
        ////{ return GetHelper<ITypeMemberCommentWhite>(); }
        ////public static RDomFactoryHelper<IStatementCommentWhite> GetHelperForStatement()
        ////{ return GetHelper<IStatementCommentWhite>(); }
        ////public static RDomFactoryHelper<IExpression> GetHelperForExpression()
        ////{ return GetHelper<IExpression>(); }
        ////public static RDomFactoryHelper<IMisc> GetHelperForMisc()
        ////{ return GetHelper<IMisc>(); }

        //// I have not proven this cache to be necessary or useful, and I don't think we can get around a cast 
        //private IDictionary<Type, object> factoryCache = new Dictionary<Type, object>();
        //private IEnumerable<TKind> GetFromFactory<TKind>(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        //{
        //    object testFactory;
        //    IRDomFactory factory = null;
        //    var type = typeof(TKind);
        //    if (factoryCache.TryGetValue(type, out testFactory))
        //    { factory = (IRDomFactory)testFactory; }
        //    else
        //    {
        //        // factory = factoryProvider.GetFactory<TKind>();
        //        //factoryCache.Add(type, factory);
        //    }
        //    return factory.CreateFrom(syntaxNode, parent, model).Cast<TKind>();
        //}

        //public IEnumerable<IPublicAnnotation> GetPublicAnnotations(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        //{ return GetFromFactory<IPublicAnnotation>(syntaxNode, parent, model); }
        //public IEnumerable<IStructuredDocumentation> GetStructuredDocumentation(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        //{ return GetFromFactory<IStructuredDocumentation>(syntaxNode, parent, model); }
        //public IEnumerable<ICommentWhite> GetCommentWhite(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        //{ return GetFromFactory<ICommentWhite>(syntaxNode, parent, model); }
        //public IEnumerable<IAttribute> CreateAttributeFrom(SyntaxNode syntaxnode, IDom parent, SemanticModel model)
        //{ return GetFromFactory<IAttribute>(syntaxnode, parent, model); }

        ////public IEnumerable<IAttribute> CreateAttributeFrom(SyntaxNode attributeNode, IDom parent, SemanticModel model)
        ////{
        ////    if (!factoryProvider.isLoaded) { factoryProvider.Initialize(registration); }
        ////    var factory = factoryProvider.GetFactory<IAttribute>();
        ////    return factory.CreateFrom(attributeNode, parent, model);
        ////}

        ////public IEnumerable<SyntaxNode>BuildAttributeSyntax(AttributeList attributes)
        ////{
        ////    if (!factoryProvider.isLoaded) { factoryProvider.Initialize(registration); }
        ////    var factory = factoryProvider.GetFactory<IAttribute>();
        ////    var ret = new List<SyntaxNode>();
        ////    foreach (var attr in attributes)
        ////    {
        ////        var node = factory.BuildSyntax(attr);
        ////        ret.AddRange(node);
        ////    }
        ////    return ret;
        ////}

        ////protected static IEnumerable<IRDomFactory<T>> GetFactories<T>()
        ////{
        ////    if (!factoryProvider.isLoaded) { factoryProvider.Initialize(registration); }
        ////    return factoryProvider.GetFactories<T>();
        ////}

        ////public virtual SyntaxNode BuildSyntax(IDom item)
        ////{ return BuildSyntaxGroup(item).Single(); }

        ////public abstract IEnumerable<SyntaxNode> BuildSyntaxGroup(IDom item);

    }

    internal class FactorySet
    {
        private IEnumerable<IRDomFactory> factories;
        private IRDomFactory genericFactory;
        private Type typeKind;
        //private IEnumerable<Tuple<IRDomFactory, Type, Type>> factoryLookup;

        internal FactorySet(Type typeKind, IEnumerable<IRDomFactory> factories, IRDomFactory genericFactory)
        {
            this.factories = factories;
            this.typeKind = typeKind;
            this.genericFactory = genericFactory;
        }

        //protected IEnumerable<IRDomFactory<TKind>> Factories
        //{
        //    get
        //    {
        //        if (factories == null) { factories = GetFactories<TKind>(); }
        //        return factories;
        //    }
        //}

        //private IEnumerable<Tuple<IRDomFactory, Type, Type>> FactoryLookup
        //{
        //    get
        //    {
        //        if (factoryLookup == null)
        //        {
        //            var list = new List<Tuple<IRDomFactory, Type, Type>>();
        //            foreach (var factory in factories)
        //            {
        //                var newTuple = Tuple.Create
        //                        (factory, GetSyntaxType(factory), GetTargetType(factory));
        //                list.Add(newTuple);
        //            }
        //            factoryLookup = list;
        //        }
        //        return factoryLookup;
        //    }
        //}

        //private Type GetSyntaxType(IRDomFactory factory)
        //{
        //    var factoryType = factory.GetType();
        //    var syntaxType = RoslynDomUtilities.FindFirstSyntaxNodeType(factoryType);
        //    return syntaxType;
        //}

        //private Type GetTargetType(IRDomFactory factory)
        //{
        //    var factoryType = factory.GetType();
        //    var syntaxType = RoslynDomUtilities.FindFirstIDomType(factoryType);
        //    return syntaxType;
        //}

        //internal IEnumerable<IRDomFactory> GetCandidates(IDom item)
        //{
        //    var type = item.GetType();
        //    var found = FactoryLookup
        //                    .Where(x => x.Item3.IsAssignableFrom(type))
        //                    .Select(x => x.Item1);
        //    return found;
        //}

        internal IEnumerable<IRDomFactory> Factories
        { get { return factories; } }

        //internal IEnumerable<IRDomFactory> GetCandidates(SyntaxNode node)
        //{
        //    var type = node.GetType();
        //    var found = factories
        //                      .Where(x => x.GetType().IsAssignableFrom(type))
        //                      .Select(x => x);
        //    return found;
        //}


    }
}

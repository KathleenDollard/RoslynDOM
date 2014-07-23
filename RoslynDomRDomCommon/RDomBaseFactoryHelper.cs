using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)

    public abstract class RDomFactoryHelper
    {
        // WARNING: At present you must register all factories before retrieving any. 
        private static FactoryProvider factoryProvider = new FactoryProvider();
        private static List<Tuple<Type, RDomFactoryHelper>> registration = new List<Tuple<Type, RDomFactoryHelper>>();

        public static void Register<TKind>(RDomFactoryHelper<TKind> factoryHelper)
            where TKind : class, IDom
        {
            registration.Add(new Tuple<Type, RDomFactoryHelper>(typeof(TKind), factoryHelper));
        }

        private static RDomFactoryHelper<TKind> GetHelper<TKind>()
            where TKind : class, IDom
        {
            foreach (var tuple in registration)
            {
                if (tuple.Item1 == typeof(TKind)) { return (RDomFactoryHelper<TKind>)tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        // Added these because of bugs with random TKind - wrong or without CommentWhite - causing missing factories - which is hard to recover from
        public static RDomFactoryHelper<IRoot> GetHelperForRoot()
        { return GetHelper<IRoot>(); }
        public static RDomFactoryHelper<IStemMemberCommentWhite> GetHelperForStemMember()
        { return GetHelper<IStemMemberCommentWhite>(); }
        public static RDomFactoryHelper<ITypeMemberCommentWhite> GetHelperForTypeMember()
        { return GetHelper<ITypeMemberCommentWhite>(); }

        public static RDomFactoryHelper<IStatementCommentWhite> GetHelperForStatement()
        { return GetHelper<IStatementCommentWhite>(); }
        public static RDomFactoryHelper<IExpression> GetHelperForExpression()
        { return GetHelper<IExpression>(); }
        public static RDomFactoryHelper<IMisc> GetHelperForMisc()
        { return GetHelper<IMisc>(); }

        // I have not proven this cache to be necessary or useful, and I don't think we can get around a cast 
        private static IDictionary<Type, object> factoryCache = new Dictionary<Type, object>();
        public static IEnumerable<TKind> GetFromFactory<TKind>(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            object testFactory;
            IRDomFactory<TKind> factory;
            var type = typeof(TKind);
            if (factoryCache.TryGetValue(type, out testFactory))
            { factory = (IRDomFactory<TKind>)testFactory; }
            else
            {
                if (!factoryProvider.isLoaded) { factoryProvider.Initialize(registration); }
                factory = factoryProvider.GetFactory<TKind>();
                factoryCache.Add(type, factory);
            }
            return factory.CreateFrom(syntaxNode, parent, model);
        }

        public static IEnumerable<IPublicAnnotation> GetPublicAnnotations(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {            return GetFromFactory<IPublicAnnotation>(syntaxNode, parent, model);        }
        public static IEnumerable<IStructuredDocumentation> GetStructuredDocumentation(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        { return GetFromFactory<IStructuredDocumentation>(syntaxNode, parent, model); }
        public static IEnumerable<ICommentWhite> GetCommentWhite(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        { return GetFromFactory<ICommentWhite>(syntaxNode, parent, model); }
        public static IEnumerable<IAttribute> CreateAttributeFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        { return GetFromFactory<IAttribute>(syntaxNode, parent, model); }

        //    public static IEnumerable<IAttribute> CreateAttributeFrom(SyntaxNode attributeNode, IDom parent, SemanticModel model)
        //{
        //    if (!factoryProvider.isLoaded) { factoryProvider.Initialize(registration); }
        //    var factory = factoryProvider.GetFactory<IAttribute>();
        //    return factory.CreateFrom(attributeNode, parent, model);
        //}

        public static IEnumerable<SyntaxNode> BuildAttributeSyntax(AttributeList attributes)
        {
            if (!factoryProvider.isLoaded) { factoryProvider.Initialize(registration); }
            var factory = factoryProvider.GetFactory<IAttribute>();
            var ret = new List<SyntaxNode>();
            foreach (var attr in attributes)
            {
                var node = factory.BuildSyntax(attr);
                ret.AddRange(node);
            }
            return ret;
        }

        protected static IEnumerable<IRDomFactory<T>> GetFactories<T>()
        {
            if (!factoryProvider.isLoaded) { factoryProvider.Initialize(registration); }
            return factoryProvider.GetFactories<T>();
        }

        public abstract SyntaxNode BuildSyntax(IDom item);
        public abstract IEnumerable<SyntaxNode> BuildSyntaxGroup(IDom item);

    }

    public abstract class RDomFactoryHelper<T> : RDomFactoryHelper
        where T : class, IDom
    {
        private IEnumerable<IRDomFactory<T>> factories;
        private IRDomFactory<T> genericFactory;
        private IEnumerable<Tuple<IRDomFactory<T>, Type, Type>> factoryLookup;

        protected IEnumerable<IRDomFactory<T>> Factories
        {
            get
            {
                if (factories == null) { factories = GetFactories<T>(); }
                return factories;
            }
        }

        private IEnumerable<Tuple<IRDomFactory<T>, Type, Type>> FactoryLookup
        {
            get
            {
                if (factoryLookup == null)
                {
                    var list = new List<Tuple<IRDomFactory<T>, Type, Type>>();
                    foreach (var factory in Factories)
                    {
                        var newTuple = new Tuple<IRDomFactory<T>, Type, Type>
                                (factory, GetSyntaxType(factory), GetTargetType(factory));
                        list.Add(newTuple);
                    }
                    factoryLookup = list;
                }
                return factoryLookup;
            }
        }

        private Type GetSyntaxType(IRDomFactory<T> factory)
        {
            var factoryType = factory.GetType();
            var syntaxType = RoslynDomUtilities.FindFirstSyntaxNodeType(factoryType);
            return syntaxType;
        }

        private Type GetTargetType(IRDomFactory<T> factory)
        {
            var factoryType = factory.GetType();
            var syntaxType = RoslynDomUtilities.FindFirstIDomType(factoryType);
            return syntaxType;
        }


        protected IRDomFactory<T> GetFactory(T item)
        {
            var type = item.GetType();
            var found = FactoryLookup.Where(x => x.Item3 == type).FirstOrDefault();
            return found.Item1;
        }

        public IEnumerable<T> MakeItems(SyntaxNode rawStatement, IDom parent, SemanticModel model)
        {
            var factories = Factories.OrderByDescending(x => x.Priority).ToArray();
            foreach (var factory in factories)
            {
                if (factory.CanCreateFrom(rawStatement))
                {
                    return factory.CreateFrom(rawStatement, parent, model);
                }
            }
            return null;
        }

        public override IEnumerable<SyntaxNode> BuildSyntaxGroup(IDom item)
        {
            if (item == null) return null;
            var itemAsT = item as T;
            if (itemAsT == null) throw new InvalidOperationException();
            var factory = GetFactory(itemAsT);
            var ret = factory.BuildSyntax(itemAsT);
            return ret;
        }

        public override SyntaxNode BuildSyntax(IDom item)
        {
            return BuildSyntaxGroup(item).Single();
        }
    }


}

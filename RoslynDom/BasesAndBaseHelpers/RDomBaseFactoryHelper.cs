using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)

    public abstract class RDomFactoryHelper
    {
        private static FactoryProvider factoryProvider = new FactoryProvider();
        private static IRDomFactory<PublicAnnotation> publicAnnotationFactory;
        private static RDomRootFactoryHelper rootFactoryHelper;
        private static RDomStemMemberFactoryHelper stemMemberFactoryHelper;
        private static RDomTypeMemberFactoryHelper typeMemberFactoryHelper;
        private static RDomStatementFactoryHelper statementFactoryHelper;
        private static RDomExpressionFactoryHelper expressionFactoryHelper;
        private static RDomMiscFactoryHelper miscFactoryHelper;

        public static void Initialize()
        {
            publicAnnotationFactory = factoryProvider.GetPublicAnnotationFactory();
        }

        public static RDomRootFactoryHelper RootFactoryHelper
        {
            get
            {
                if (rootFactoryHelper == null) { rootFactoryHelper = new RDomRootFactoryHelper(); }
                return rootFactoryHelper;
            }
        }
        public static RDomStemMemberFactoryHelper StemMemberFactoryHelper
        {
            get
            {
                if (stemMemberFactoryHelper == null) { stemMemberFactoryHelper = new RDomStemMemberFactoryHelper(); }
                return stemMemberFactoryHelper;
            }
        }
        public static RDomTypeMemberFactoryHelper TypeMemberFactoryHelper
        {
            get
            {
                if (typeMemberFactoryHelper == null) { typeMemberFactoryHelper = new RDomTypeMemberFactoryHelper(); }
                return typeMemberFactoryHelper;
            }
        }
        public static RDomStatementFactoryHelper StatementFactoryHelper
        {
            get
            {
                if (statementFactoryHelper == null) { statementFactoryHelper = new RDomStatementFactoryHelper(); }
                return statementFactoryHelper;
            }
        }
        public static RDomExpressionFactoryHelper ExpressionFactoryHelper
        {
            get
            {
                if (expressionFactoryHelper == null) { expressionFactoryHelper = new RDomExpressionFactoryHelper(); }
                return expressionFactoryHelper;
            }
        }
        public static RDomMiscFactoryHelper MiscFactoryHelper
        {
            get
            {
                if (miscFactoryHelper == null) { miscFactoryHelper = new RDomMiscFactoryHelper(); }
                return miscFactoryHelper;
            }
        }

        public static RDomFactoryHelper GetHelper<TKind>()
        {
            if (typeof(TKind) == typeof(IRoot)) { return RootFactoryHelper; }
            if (typeof(TKind) == typeof(IStemMember)) { return StemMemberFactoryHelper; }
            if (typeof(TKind) == typeof(ITypeMember)) { return TypeMemberFactoryHelper; }
            if (typeof(TKind) == typeof(IStatement)) { return  StatementFactoryHelper; }
            if (typeof(TKind) == typeof(IExpression)) { return ExpressionFactoryHelper; }
            if (typeof(TKind) == typeof(IMisc)) { return MiscFactoryHelper; }
            throw new InvalidOperationException();
        }

        public static IEnumerable<PublicAnnotation> GetPublicAnnotations(SyntaxNode syntaxNode)
        {
            if (publicAnnotationFactory == null) Initialize();
            return publicAnnotationFactory.CreateFrom(syntaxNode);
        }

        protected static IEnumerable<IRDomFactory<T>> GetFactories<T>()
        {
            return factoryProvider.GetFactories<T>();
        }

        public abstract IEnumerable<SyntaxNode> BuildSyntax(IDom item);
   
    }

    public abstract class RDomFactoryHelper<T> : RDomFactoryHelper
    {
        private IEnumerable<IRDomFactory<T>> factories;
        private IRDomFactory<T> genericFactory;

        protected IEnumerable<IRDomFactory<T>> Factories
        {
            get
            {
                if (factories == null) { factories = GetFactories<T>(); }
                return factories;
            }
        }

    }

    public abstract class RDomFactoryHelper<T, TSyntax> : RDomFactoryHelper<T>
        where TSyntax : SyntaxNode
    {
        public IEnumerable<T> MakeItem(TSyntax rawStatement)
        {
            foreach (var factory in Factories)
            {
                if (factory.CanCreateFrom(rawStatement))
                {
                    return factory.CreateFrom(rawStatement);
                }
            }
            return null;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            throw new NotImplementedException();
        }
    }

    public class RDomRootFactoryHelper : RDomFactoryHelper<IRoot, CompilationUnitSyntax>
    { internal RDomRootFactoryHelper() { } }

    public class RDomStemMemberFactoryHelper : RDomFactoryHelper<IStemMember, SyntaxNode>
    { internal RDomStemMemberFactoryHelper() { } }


    public class RDomTypeMemberFactoryHelper : RDomFactoryHelper<ITypeMember, MemberDeclarationSyntax>
    { internal RDomTypeMemberFactoryHelper() { } }


    public class RDomStatementFactoryHelper : RDomFactoryHelper<IStatement, StatementSyntax>
    { internal RDomStatementFactoryHelper() { } }


    public class RDomExpressionFactoryHelper : RDomFactoryHelper<IExpression, ExpressionSyntax>
    { internal RDomExpressionFactoryHelper() { } }

    public class RDomMiscFactoryHelper : RDomFactoryHelper<IMisc, SyntaxNode>
    { internal RDomMiscFactoryHelper() { } }

}

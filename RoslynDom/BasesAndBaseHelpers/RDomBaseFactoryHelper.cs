using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Practices.Unity;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomFactoryHelper
    {
        private IPublicAnnotationFactory publicAnnotationFactory;

        public RDomFactoryHelper(IUnityContainer container)
        {
            publicAnnotationFactory = container.ResolveAll<IPublicAnnotationFactory>().FirstOrDefault();

        }
        public IEnumerable<PublicAnnotation> GetPublicAnnotations(SyntaxNode syntaxNode)
        {
            return publicAnnotationFactory.CreateFrom(syntaxNode);
        }
    }

    public class RDomFactoryHelper<T> : RDomFactoryHelper
    {
        private IEnumerable<IRDomFactory<T>> factories;

        public RDomFactoryHelper(IUnityContainer container) : base(container)
        {
            factories = container
                .ResolveAll<IRDomFactory<T>>();
        }

        protected IEnumerable<IRDomFactory<T>> Factories
        { get { return factories; } }
  
    }

    public class RDomFactoryHelper<T, TSyntax> : RDomFactoryHelper<T>
        where TSyntax : SyntaxNode
    {
        public RDomFactoryHelper(IUnityContainer container) : base(container)
        {   }

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
    }

    public class RDomStemMemberFactoryHelper : RDomFactoryHelper<IStemMember, SyntaxNode>
    {
        public RDomStemMemberFactoryHelper(IUnityContainer container) : base(container)
        { }
    }

    public class RDomTypeMemberFactoryHelper : RDomFactoryHelper<ITypeMember, MemberDeclarationSyntax>
    {
        public RDomTypeMemberFactoryHelper(IUnityContainer container) : base(container)
        { }
    }

    public class RDomStatementFactoryHelper : RDomFactoryHelper<IStatement, StatementSyntax>
    {
        public RDomStatementFactoryHelper(IUnityContainer container) : base(container)
        { }
    }
}

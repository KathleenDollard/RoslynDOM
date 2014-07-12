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

    public abstract class RDomFactoryBase<T, TSyntax, TKind, THelper> : IRDomFactory<TKind>
        where T : TKind
        where TSyntax : SyntaxNode
        where THelper : RDomFactoryHelper
    {
        private THelper factoryHelper;
        public RDomFactoryBase(RDomFactoryHelper helper)
        {
            this.factoryHelper = (THelper)(helper as object);
        }

        public virtual FactoryPriority Priority
        { get { return FactoryPriority.Normal; } }

        public virtual bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return (syntaxNode is TSyntax);
        }

        public virtual IEnumerable<TKind> CreateFrom(SyntaxNode syntaxNode)
        {
            var publicAnnotations = factoryHelper.GetPublicAnnotations(syntaxNode);
            var newStatement = Activator.CreateInstance(
                        typeof(T),
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
                        new object[] { (TSyntax)syntaxNode, publicAnnotations }, null);
            return new TKind[] { (TKind)newStatement };
        }
    }

    public abstract class RDomStemMemberFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IStemMember , RDomStemMemberFactoryHelper>
            where T : IStemMember
            where TSyntax : SyntaxNode
    {
        public RDomStemMemberFactory(RDomFactoryHelper helper) : base(helper)
        { }
    }

    public abstract class RDomTypeMemberFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, ITypeMember, RDomTypeMemberFactoryHelper>
            where T : ITypeMember
            where TSyntax : SyntaxNode
    {
        public RDomTypeMemberFactory(RDomFactoryHelper helper) : base(helper)
        { }
    }

    public abstract class RDomStatementFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IStatement, RDomStatementFactoryHelper>
            where T : IStatement
            where TSyntax : SyntaxNode
    {
        public RDomStatementFactory(RDomFactoryHelper helper) : base(helper)
        {    }
    }
}

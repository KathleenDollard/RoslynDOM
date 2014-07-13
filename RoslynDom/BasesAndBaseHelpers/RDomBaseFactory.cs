using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    // Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
    public abstract class RDomFactoryBase<T, TSyntax, TKind> : IRDomFactory<TKind>
        where T : TKind
        where TSyntax : SyntaxNode
    {
        public RDomFactoryBase()
        { }

        public virtual FactoryPriority Priority
        { get { return FactoryPriority.Normal; } }

        public abstract IEnumerable<SyntaxNode> BuildSyntax(TKind item);
        //{
        //    return null;
        //}

        public virtual bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return (syntaxNode is TSyntax);
        }

        public virtual IEnumerable<TKind> CreateFrom(SyntaxNode syntaxNode)
        {
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(syntaxNode);
            var newStatement = Activator.CreateInstance(
                        typeof(T),
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
                        new object[] { (TSyntax)syntaxNode }, null);
            return new TKind[] { (TKind)newStatement };
        }
    }

    public  class RDomRootContainerFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IRoot>
             where T : IRoot
             where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IRoot item)
        { return null; }
    }

    public class RDomStemMemberFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IStemMember>
            where T : IStemMember
            where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        { return null;  }
    }

    public  class RDomTypeMemberFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, ITypeMember>
            where T : ITypeMember
            where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        { return null; }
    }

    public  class RDomStatementFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IStatement>
            where T : IStatement
            where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        { return null; }
    }

    public  class RDomExpressionFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IExpression>
             where T : IExpression
             where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IExpression item)
        { return null; }
    }

    public  class RDomMiscFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IMisc>
             where T : IMisc
             where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IMisc item)
        { return null; }
    }
}

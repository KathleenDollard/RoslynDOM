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
        where T : TKind  // This isn't stupid - TKind is the broader category, T is the specific 
        where TSyntax : SyntaxNode
        where TKind : class
    {
        public RDomFactoryBase()
        { }

        public virtual FactoryPriority Priority
        {
            get
            {
                if (this.GetType().IsConstructedGenericType) { return FactoryPriority.Fallback; }
                return FactoryPriority.Normal;
            }
        }

        public abstract IEnumerable<SyntaxNode> BuildSyntax(TKind item);
       
        public virtual bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return (syntaxNode is TSyntax);
        }

        public virtual IEnumerable<TKind> CreateFrom(SyntaxNode syntaxNode)
        {
            var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(syntaxNode);
            var newItem = Activator.CreateInstance(
                        typeof(T),
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
                        new object[] { (TSyntax)syntaxNode }, null);
            var itemAsTKind = newItem as TKind;
            InitializeItem(itemAsTKind);
            return new TKind[] { itemAsTKind };
        }

        public virtual void InitializeItem(TKind newItem)
        { return; }
    }

    public class RDomRootContainerFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IRoot>
             where T : IRoot
             where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IRoot item)
        {
            throw new NotImplementedException();
        }
    }

    public class RDomStemMemberFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IStemMember>
            where T : IStemMember
            where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            throw new NotImplementedException();
        }
    }

    public class RDomTypeMemberFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, ITypeMember>
            where T : ITypeMember
            where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            throw new NotImplementedException();
        }
    }

    public class RDomStatementFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IStatement>
            where T : IStatement
            where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
        {
            throw new NotImplementedException();
        }
    }

    public class RDomExpressionFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IExpression>
             where T : IExpression
             where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IExpression item)
        {
            throw new NotImplementedException();
        }
    }

    public class RDomMiscFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IMisc>
             where T : IMisc
             where TSyntax : SyntaxNode
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IMisc item)
        {
            throw new NotImplementedException();
        }
    }
}

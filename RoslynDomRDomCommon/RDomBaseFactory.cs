using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    //// Factories are specific to the type, FactoryHelpers are specific to the level (StemMember, TypeMember, Statement, Expression)
    //public abstract class RDomFactoryBase<T, TSyntax, TKind> : IRDomFactory
    //    where T : class, TKind  // This isn't stupid - TKind is the broader category, T is the specific 
    //    where TSyntax : SyntaxNode
    //    where TKind : class
    //{
    //    public RDomFactoryBase()
    //    { }

    //    public virtual RDomPriority Priority
    //    {
    //        get
    //        {
    //            if (this.GetType().IsConstructedGenericType) { return RDomPriority.Fallback; }
    //            return RDomPriority.Normal;
    //        }
    //    }

    //    public abstract IEnumerable<SyntaxNode> BuildSyntax(IDom item);

    //    public virtual bool CanCreateFrom(SyntaxNode syntaxNode)
    //    {
    //        return (syntaxNode is TSyntax);
    //    }

    //    public virtual bool CanBuildSyntax(IDom item)
    //    {
    //        // TODO: use this to call correct IStem or IType version of class, interface, structure and enum
    //        return true;
    //    }

    //    public abstract IEnumerable<IDom> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model);

    //    public virtual void InitializeItem(T newItem, TSyntax syntax, IDom parent, SemanticModel model)
    //    { return; }
    //}

    ////public abstract class RDomRootContainerFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IRoot>
    ////         where T : class, IRoot
    ////         where TSyntax : SyntaxNode
    ////{
    ////    public override IEnumerable<SyntaxNode> BuildSyntax(IRoot item)
    ////    { throw new NotImplementedException(); }
    ////}

    ////public abstract class RDomStemMemberFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IStemMember>
    ////        where T : class, IStemMember
    ////        where TSyntax : SyntaxNode
    ////{

    ////    public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
    ////    { throw new NotImplementedException(); }
    ////}

    ////public abstract class RDomTypeMemberFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, ITypeMember>
    ////        where T : class, ITypeMember
    ////        where TSyntax : SyntaxNode
    ////{
    ////    public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
    ////    { throw new NotImplementedException(); }
    ////}

    ////public abstract class RDomStatementFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IStatement>
    ////        where T : class, IStatement
    ////        where TSyntax : SyntaxNode
    ////{
    ////    public override IEnumerable<SyntaxNode> BuildSyntax(IStatement item)
    ////    { throw new NotImplementedException(); }
    ////}

    ////public class RDomExpressionFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IExpression>
    ////         where T : class, IExpression
    ////         where TSyntax : SyntaxNode
    ////{
    ////    public override IEnumerable<SyntaxNode> BuildSyntax(IExpression item)
    ////    { throw new NotImplementedException(); }

    ////    public override IEnumerable<IExpression> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
    ////    {
    ////        var syntax = syntaxNode as TSyntax;
    ////        //var publicAnnotations = RDomFactoryHelper.GetPublicAnnotations(syntaxNode);
    ////        var newItem = Activator.CreateInstance(
    ////                    typeof(T),
    ////                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null,
    ////                    new object[] { syntax, parent, model }, null);
    ////        var itemAsT = newItem as T;
    ////        InitializeItem(itemAsT, syntax, parent, model);
    ////        return new IExpression[] { itemAsT };
    ////    }
    ////}

    ////public abstract class RDomMiscFactory<T, TSyntax> : RDomFactoryBase<T, TSyntax, IMisc>
    ////         where T : class, IMisc
    ////         where TSyntax : SyntaxNode
    ////{
    ////    public override IEnumerable<SyntaxNode> BuildSyntax(IMisc item)
    ////    { throw new NotImplementedException(); }
    ////}
}

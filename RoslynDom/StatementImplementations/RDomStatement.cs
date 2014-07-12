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
    public class RDomStatementFactoryHelper : RDomFactoryHelper<IStatement>
    {
        public RDomStatementFactoryHelper(IUnityContainer container) : base(container)
        { }

        public IEnumerable<IStatement> MakeStatement(StatementSyntax rawStatement)
        {
            foreach (var statementFactory in Factories)
            {
                if (statementFactory.CanCreateFrom(rawStatement))
                {
                    return statementFactory.CreateFrom(rawStatement);
                }
            }
            return null;
        }
    }

    public abstract class RDomStatementFactory<T, TSyntax> : IRDomFactory<IStatement>
            where T : IStatement
            where TSyntax : SyntaxNode
    {
        private RDomStatementFactoryHelper factoryHelper;
        public RDomStatementFactory(RDomFactoryHelper helper)
        {
            this.factoryHelper = (RDomStatementFactoryHelper)(helper as object);
        }

        public virtual FactoryPriority Priority
        { get { return FactoryPriority.Normal; } }

        public virtual bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return (syntaxNode is TSyntax);
        }

        public virtual IEnumerable<IStatement> CreateFrom(SyntaxNode syntaxNode)
        {
            var publicAnnotations = factoryHelper.GetPublicAnnotations(syntaxNode);
            var newStatement = Activator.CreateInstance(
                        typeof(T), 
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, 
                        new object[] { (TSyntax)syntaxNode, publicAnnotations}, null);
            return new IStatement[] { (IStatement)newStatement };
        }
    }

    public class RDomStatement : RDomBase<IStatement, StatementSyntax, ISymbol>, IStatement
    {

        internal RDomStatement(
            StatementSyntax rawItem,
            StatementKind statementKind,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        { StatementKind = statementKind; }

        internal RDomStatement(
             StatementSyntax rawItem,
             StatementKind statementKind,
             IEnumerable<PublicAnnotation> publicAnnotations)
           : base(rawItem, publicAnnotations)
        { StatementKind = statementKind; }

        internal RDomStatement(RDomStatement oldRDom)
             : base(oldRDom)
        {
            StatementKind = oldRDom.StatementKind;
        }

        public override StatementSyntax BuildSyntax()
        {
            return TypedSyntax;
        }

        public StatementKind StatementKind { get; set; }


    }
}

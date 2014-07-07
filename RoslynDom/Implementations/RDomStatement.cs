using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStatement : RDomBase<IStatement, StatementSyntax, ISymbol>, IStatement
    {
        private IList<IStatement> _statements = new List<IStatement>();

        internal RDomStatement(
            StatementSyntax rawItem,
            IEnumerable<IStatement> statements,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            foreach (var statement in statements)
            { AddStatement(statement); }
            Initialize();
        }

        internal RDomStatement(RDomStatement oldRDom)
             : base(oldRDom)
        {
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            foreach (var statement in newStatements)
            { AddStatement(statement); }

            IsBlock = oldRDom.IsBlock;
            StatementKind = oldRDom.StatementKind;
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsBlock = _statements.Count() > 1 || TypedSyntax is BlockSyntax;
        }

        public override StatementSyntax BuildSyntax()
        {
            return TypedSyntax;
        }
    
        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddStatement(IStatement statement)
        { _statements.Add(statement); }

  
        public bool IsBlock { get; set; }
       
        public StatementKind StatementKind { get; set; }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }
       
    }
}

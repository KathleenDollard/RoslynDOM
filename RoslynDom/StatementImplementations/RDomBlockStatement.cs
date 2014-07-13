using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
   public class RDomBlockStatementFactory
        : RDomStatementFactory<RDomBlockStatement, BlockSyntax>
    { }


    public class RDomBlockStatement : RDomBase<IBlockStatement, BlockSyntax, ISymbol>, IBlockStatement
    {
        private IList<IStatement> _statements = new List<IStatement>();

        internal RDomBlockStatement(BlockSyntax rawItem)
           : base(rawItem)
        {
            Initialize2();
        }

        internal RDomBlockStatement(
            BlockSyntax rawItem,
              IEnumerable<PublicAnnotation> publicAnnotations)
          : base(rawItem,   publicAnnotations)
        {
            Initialize();
        }

     
        internal RDomBlockStatement(RDomBlockStatement oldRDom)
             : base(oldRDom)
        { }

 
        protected override void Initialize()
        {
            base.Initialize();
        }

       protected void Initialize2()
        {
            Initialize();
        }

        public override BlockSyntax BuildSyntax()
        {
            return null;
        }


        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddOrMoveStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements; } }
    }
}

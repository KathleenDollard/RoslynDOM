using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomBlockStatement : RDomBaseBlock, IBlockStatement
    {

        internal RDomBlockStatement(
            BlockSyntax rawItem,
            IEnumerable<IStatement> statements,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, statements, StatementKind.Block, publicAnnotations)
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

        public override StatementSyntax BuildSyntax()
        {
            return null;
        }

    }
}

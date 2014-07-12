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

        internal RDomStatement(
            StatementSyntax rawItem,
            StatementKind statementKind,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        { StatementKind = statementKind;    }

        internal RDomStatement(
             StatementSyntax rawItem,
             StatementKind statementKind,
             IEnumerable< PublicAnnotation> publicAnnotations)
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

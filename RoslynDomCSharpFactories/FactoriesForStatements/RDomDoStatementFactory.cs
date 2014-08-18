using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomDoStatementFactory
         : RDomStatementFactory<RDomDoStatement, DoStatementSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomDoStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as DoStatementSyntax;
            var newItem = new RDomDoStatement(syntaxNode, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

            newItem.TestAtEnd = true; 
            return LoopFactoryHelper.CreateItemFrom<IDoStatement>(newItem, syntax.Condition, syntax.Statement, parent, model, Corporation, CreateFromWorker  );
        }

          public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IDoStatement ;
            return LoopFactoryHelper.BuildSyntax<IDoStatement>(itemAsT, (c, s) => SyntaxFactory.DoStatement(s, c), WhitespaceLookup);
        }

 
    }
}

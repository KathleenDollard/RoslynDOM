using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomForEachStatementFactory
         : RDomBaseLoopStatementFactory<RDomForEachStatement, ForEachStatementSyntax>
    {
        //private static WhitespaceKindLookup _whitespaceLookup;

        public RDomForEachStatementFactory(RDomCorporation corporation)
         : base(corporation)
        { }

        //private WhitespaceKindLookup WhitespaceLookup
        //{
        //    get
        //    {
        //        if (_whitespaceLookup == null)
        //        {
        //            _whitespaceLookup = new WhitespaceKindLookup();
        //            _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
        //        }
        //        return _whitespaceLookup;
        //    }
        //}

        protected override IStatementCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ForEachStatementSyntax;
            var newItem = base.CreateItemFrom(syntaxNode, parent, model) as RDomForEachStatement;
            newItem.TestAtEnd = false; // restating the default
            var newVariable = Corporation.CreateFrom<IMisc>(syntaxNode, newItem, model).FirstOrDefault();
            newItem.Variable = (IVariableDeclaration)newVariable;
            return newItem;
            //var syntax = syntaxNode as ForEachStatementSyntax;
            //var newItem = new RDomForEachStatement(syntaxNode, parent, model);
            //CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            //CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

            //newItem.TestAtEnd = false; // restating the obvious

            //var newVariable = Corporation.CreateFrom<IMisc>(syntaxNode, newItem, model).FirstOrDefault();
            //newItem.Variable = (IVariableDeclaration)newVariable;
            ////var variable = new RDomVariableDeclaration(syntaxNode, parent, model); // ick, there is no syntax node to associate with the variable
            ////variable.IsImplicitlyTyped = (syntax.Type.ToString() == "var");
            ////var typeSymbol = ((ILocalSymbol)newItem.TypedSymbol).Type; 
            ////variable.Type = new RDomReferencedType(typeSymbol.DeclaringSyntaxReferences, typeSymbol);
            ////variable.Name =syntax.Identifier.ToString();
            ////newItem.Variable = variable;
            //return LoopFactoryHelper.CreateItemFrom<IForEachStatement>(newItem, 
            //    syntax.Expression, syntax.Statement, parent,  model,
            //        Corporation, CreateFromWorker, WhitespaceLookup);
        }

        protected override ExpressionSyntax GetConditionFromSyntax(ForEachStatementSyntax syntax)
        {
            return syntax.Expression;
        }

        protected override StatementSyntax GetStatementFromSyntax(ForEachStatementSyntax syntax)
        {
            return syntax.Statement;
        }

        protected override RDomForEachStatement MakeNewItem(ForEachStatementSyntax syntax, IDom parent, SemanticModel model)
        {
            return new RDomForEachStatement(syntax, parent, model);
        }

        protected override ForEachStatementSyntax MakeSyntax(
                RDomForEachStatement itemAsT, ExpressionSyntax condition, StatementSyntax statementBlock)
        {
            var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT.Variable);
            typeSyntax = BuildSyntaxHelpers.AttachWhitespace(typeSyntax, itemAsT.Whitespace2Set, WhitespaceLookup);
            var id = itemAsT.Variable.Name;
            return SyntaxFactory.ForEachStatement(typeSyntax, id, condition, statementBlock);
        }


        //public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        //{
        //    var itemAsT = item as IForEachStatement;

        //    var typeSyntax = BuildSyntaxWorker.GetVariableTypeSyntax(itemAsT.Variable);
        //    //TypeSyntax typeSyntax;
        //    //if (itemAsT.Variable.IsImplicitlyTyped)
        //    //{ typeSyntax = SyntaxFactory.IdentifierName("var"); }
        //    //else
        //    //{ typeSyntax = (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(itemAsT.Variable.Type)); }

        //    return LoopFactoryHelper.BuildSyntax<IForEachStatement>
        //        (itemAsT, (c, s) => SyntaxFactory.ForEachStatement(typeSyntax, itemAsT.Variable.Name, c,s),WhitespaceLookup) ;

        //}


    }
}

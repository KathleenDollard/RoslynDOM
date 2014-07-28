using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public static class CreateFromHelpers
    {
        //public static IExpression GetExpression(IDom newItem, 
        //        ExpressionSyntax expressionSyntax, SemanticModel model, 
        //        RDomCorporation corporation)
        //{
        //    if (expressionSyntax == null) { return null; }
        //    return corporation.CreateFrom<IExpression>(expressionSyntax, newItem, model).FirstOrDefault();
        //}


        //public static void InitializeStatements(IStatementBlock newItem, 
        //        StatementSyntax statementSytax, SemanticModel model)
        //{
        //    bool hasBlock = false;
        //    var statements = GetStatementsFromSyntax(statementSytax, newItem, ref hasBlock, model);
        //    newItem.HasBlock = hasBlock;
        //    newItem.StatementsAll.AddOrMoveRange(statements);
        //}

        //public static void LoadStemMembers(IStemContainer newItem, 
        //            IEnumerable<MemberDeclarationSyntax > memberSyntaxes, 
        //            IEnumerable<UsingDirectiveSyntax > usingSyntaxes,
        //            SemanticModel model)
        //{
        //    var usings = ListUtilities.CreateFromList(usingSyntaxes, x => RDomCorporation.GetHelperForStemMember().MakeItems(x, newItem, model));
        //    var members = ListUtilities.CreateFromList (memberSyntaxes, x => RDomCorporation.GetHelperForStemMember().MakeItems(x, newItem, model));
        //    newItem.StemMembersAll.AddOrMoveRange(usings);
        //    newItem.StemMembersAll.AddOrMoveRange(members);
        //}

        ////public static void InitializeStatements(IStatementBlock newItem,
        ////   BlockSyntax  blockSytax, SemanticModel model)
        ////{
        ////    bool hasBlock = false;
        ////    var statements = GetStatementsFromSyntax(statementSytax, newItem, ref hasBlock, model);
        ////    newItem.HasBlock = hasBlock;
        ////    newItem.StatementsAll.AddOrMoveRange(statements);
        ////}


        //public static IVariableDeclaration GetVariable(ISymbol typedSymbol, 
        //        VariableDeclaratorSyntax variableSyntax, IDom parent, SemanticModel model)
        //{
        //    var parentSyntax = variableSyntax.Parent as VariableDeclarationSyntax;
        //    if (parentSyntax == null) throw new InvalidOperationException();
        //    var variable = new RDomVariableDeclaration(variableSyntax, parent, model); 
        //    variable.IsImplicitlyTyped = (parentSyntax.Type.ToString() == "var");
        //    // not sure this is valid at all
        //    variable.Type = new RDomReferencedType(typedSymbol.DeclaringSyntaxReferences, typedSymbol);
        //    variable.Name = variableSyntax.Identifier.ToString();
        //    return  variable;
        //}

        //public static IEnumerable<IStatementCommentWhite> GetStatementsFromSyntax(StatementSyntax statementSyntax, 
        //    IDom parent, ref bool hasBlock, SemanticModel model)
        //{
        //    var statement = RDomCorporation.GetHelperForStatement().MakeItems(statementSyntax, parent, model).First();
        //    var list = new List<IStatementCommentWhite>();
        //    var blockStatement = statement as IBlockStatement;
        //    if (blockStatement != null)
        //    {
        //        hasBlock = true;
        //        foreach (var state in blockStatement.Statements)
        //        {
        //            // Don't need to copy because abandoning block
        //            list.Add(state);
        //        }
        //    }
        //    else
        //    { list.Add(statement); }
        //    return list;
        //}

    }
}

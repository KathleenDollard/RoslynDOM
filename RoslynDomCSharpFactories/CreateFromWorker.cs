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
    public class CreateFromWorker : ICSharpCreateFromWorker
    {
        public CreateFromWorker(RDomCorporation corporation)
        {
            Corporation = corporation;
        }

        protected RDomCorporation Corporation { get; private set; }

        public RDomPriority Priority
        { get { return RDomPriority.Normal; } }

        public void InitializeStatements(IStatementBlock itemAsStatement, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemAsStatement == null) return;
            var blockSyntax = syntaxNode as BlockSyntax;
            if (syntaxNode == null) return;
            if (blockSyntax != null)
            {
                var statements = ListUtilities.CreateFromList(blockSyntax.Statements, x => Corporation.CreateFrom<IStatementCommentWhite>(x, parent, model));
                itemAsStatement.StatementsAll.AddOrMoveRange(statements);
                itemAsStatement.HasBlock = true;
                return;
            }
            var statementSyntax = syntaxNode as StatementSyntax;
            if (statementSyntax != null)
            {
                var statements = Corporation.CreateFrom<IStatementCommentWhite>(statementSyntax, parent, model);
                if (statements.Count() > 1) throw new NotImplementedException();
                var statement = statements.First();
                var statementAsBlockStatement = statement as IBlockStatement;
                if (statementAsBlockStatement != null)
                {
                    itemAsStatement.HasBlock = true;
                    foreach (var st in statementAsBlockStatement.Statements )
                    { itemAsStatement.StatementsAll.AddOrMove(st); }
                }
                else
                { itemAsStatement.StatementsAll.AddOrMove(statement); }
            }
        }

        public void StandardInitialize<T>(T newItem, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
                where T : class, IDom
        {
            InitializePublicAnnotations(newItem, syntaxNode, parent, model);
            InitializeAttributes(newItem as IHasAttributes, syntaxNode, parent, model);
            InitializeAccessModifiers(newItem as IHasAccessModifier, syntaxNode, parent, model);
            InitializeStructuredDocumentation(newItem as IHasStructuredDocumentation, syntaxNode, parent, model);
            // InitializeStatements(newItem as IStatementBlock, syntaxNode, parent, model);

            //throw new NotImplementedException();
            //bool hasBlock = false;
            //var statements = CreateFromHelpers.GetStatementsFromSyntax(syntax.Statement, newItem, ref hasBlock, model);
            //newItem.HasBlock = hasBlock;
            //newItem.StatementsAll.AddOrMoveRange(statements);
        }

        public void InitializeAccessModifiers(IHasAccessModifier itemHasAccessModifier, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemHasAccessModifier == null) { return; }
            var itemAsHasSymbol = itemHasAccessModifier as IRoslynHasSymbol;
            if (itemAsHasSymbol == null) { throw new InvalidOperationException(); }
            var accessibility = itemAsHasSymbol.Symbol.DeclaredAccessibility;
            itemHasAccessModifier.AccessModifier = Mappings.GetAccessModifierFromAccessibility(accessibility);
        }

        public void InitializeAttributes(IHasAttributes itemAsHasAttributes, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (itemAsHasAttributes == null) { return; }
            var attributes = new List<IAttribute>();
            var attributeLists = syntaxNode.ChildNodes().OfType<AttributeListSyntax>();
            foreach (var attributeList in attributeLists)
            {
                if (attributeList != null)
                {
                    var attr = Corporation.CreateFrom<IAttribute>(attributeList, itemAsHasAttributes, model);
                    attributes.AddRange(attr);
                }
            }
            itemAsHasAttributes.Attributes.AddOrMoveAttributeRange(attributes);
        }

        public void InitializePublicAnnotations(IDom item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var publicAnnotations = GetPublicAnnotations(syntaxNode, item, model);
            item.PublicAnnotations.Add(publicAnnotations);
        }

        public void InitializeStructuredDocumentation(IHasStructuredDocumentation  item, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            if (item == null) return;
            var structuredDocumentation = GetStructuredDocumenation(syntaxNode, item, model).FirstOrDefault();
            if (structuredDocumentation != null)
            {
                item.StructuredDocumentation = structuredDocumentation;
                item.Description = structuredDocumentation.Description;
            }
            //    var docsItem = RDomCorporation.GetStructuredDocumentation(rawItem, parent, model).FirstOrDefault();
            //    var docs = Symbol.GetDocumentationCommentXml();
            //    if (!string.IsNullOrWhiteSpace(docs))
            //    {
            //        var xDocument = XDocument.Parse(docs);
            //        docsItem.RawItem = xDocument;
            //        var summaryNode = xDocument.DescendantNodes()
            //                            .OfType<XElement>()
            //                            .Where(x => x.Name == "summary")
            //                            .Select(x => x.Value);
            //        var description = summaryNode.FirstOrDefault().Replace("/r", "").Replace("\n", "").Trim();
            //        docsItem.Description = description;
            //        thisAsHasStructuredDocs.StructuredDocumentation = docsItem;
            //        thisAsHasStructuredDocs.Description = description;
            //    }
            //}
            //    throw new NotImplementedException();
        }

            //public static IExpression GetExpression(IDom newItem,
            //    ExpressionSyntax expressionSyntax, SemanticModel model,
            //    RDomCorporation corporation)
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

        public void LoadStemMembers(IStemContainer newItem,
                    IEnumerable<MemberDeclarationSyntax> memberSyntaxes,
                    IEnumerable<UsingDirectiveSyntax> usingSyntaxes,
                    SemanticModel model)
        {
            var usings = ListUtilities.CreateFromList(usingSyntaxes, x => Corporation.CreateFrom<IStemMemberCommentWhite>(x, newItem, model));
            var members = ListUtilities.CreateFromList(memberSyntaxes, x => Corporation.CreateFrom<IStemMemberCommentWhite>(x, newItem, model));
            newItem.StemMembersAll.AddOrMoveRange(usings);
            newItem.StemMembersAll.AddOrMoveRange(members);
        }

        public IEnumerable<ICommentWhite> GetCommentWhite<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
            where T : class, IDom
            where TSyntax : SyntaxNode
        {
            return Corporation.CreateFrom<ICommentWhite>(syntaxNode, newItem, model);
        }

        public IEnumerable<IAttribute> GetAttributes<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
            where T : class, IDom
            where TSyntax : SyntaxNode
        {
            var parentAsHasSymbol = newItem as IRoslynHasSymbol;
            if (parentAsHasSymbol == null) { throw new InvalidOperationException(); }
            var parentSymbol = parentAsHasSymbol.Symbol;
            var symbolAttributes = parentSymbol.GetAttributes();
            var list = new List<IAttribute>();
            foreach (var attributeData in symbolAttributes)
            {
                // TODO: In those cases where we do have a symbol reference to the attribute, try to use it
                var appRef = attributeData.ApplicationSyntaxReference;
                var attribSyntax = syntaxNode.SyntaxTree.GetRoot().FindNode(appRef.Span) as AttributeSyntax;
                var newAttrib = Corporation.CreateFrom<IAttribute>(attribSyntax, newItem, model);
                list.AddRange(newAttrib);
            }
            return list;
        }

        public IEnumerable<IPublicAnnotation> GetPublicAnnotations<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
            where T : class, IDom
            where TSyntax : SyntaxNode
        {
            return Corporation.CreateFrom<IPublicAnnotation>(syntaxNode, newItem, model);
        }

        public IEnumerable<IStructuredDocumentation> GetStructuredDocumenation<T, TSyntax>(TSyntax syntaxNode, T newItem, SemanticModel model)
            where T : class, IDom
            where TSyntax : SyntaxNode
        {
            return Corporation.CreateFrom<IStructuredDocumentation>(syntaxNode, newItem, model);
        }

        public IEnumerable<TKind> CreateInvalidMembers<TKind>(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
            where TKind : class
        {
            var ret = new RDomInvalidMember(syntaxNode, parent, model) as TKind;
            if (ret == null) { throw new InvalidOperationException("Invalid can't be represented as this kind"); }
            return new List<TKind>() { };
        }



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
        //    return variable;
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

        //void StandardInitialize(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        //{
        //RDomCorporation.GetPublicAnnotations(rawItem, parent, model);
        //if (thisAsHasStructuredDocs != null)
        //{
        //    var docsItem = RDomCorporation.GetStructuredDocumentation(rawItem, parent, model).FirstOrDefault();
        //    var docs = Symbol.GetDocumentationCommentXml();
        //    if (!string.IsNullOrWhiteSpace(docs))
        //    {
        //        var xDocument = XDocument.Parse(docs);
        //        docsItem.RawItem = xDocument;
        //        var summaryNode = xDocument.DescendantNodes()
        //                            .OfType<XElement>()
        //                            .Where(x => x.Name == "summary")
        //                            .Select(x => x.Value);
        //        var description = summaryNode.FirstOrDefault().Replace("/r", "").Replace("\n", "").Trim();
        //        docsItem.Description = description;
        //        thisAsHasStructuredDocs.StructuredDocumentation = docsItem;
        //        thisAsHasStructuredDocs.Description = description;
        //    }
        //}
        //    throw new NotImplementedException();
        //}


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomVariableDeclarationFactory
        : RDomMiscFactory<RDomVariableDeclaration, VariableDeclaratorSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomVariableDeclarationFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        private WhitespaceKindLookup WhitespaceLookup
        {
            get
            {
                if (_whitespaceLookup == null)
                {
                    _whitespaceLookup = new WhitespaceKindLookup();
                    _whitespaceLookup.Add(LanguageElement.Identifier, SyntaxKind.IdentifierToken);
                    _whitespaceLookup.Add(LanguageElement.AssignmentOperator, SyntaxKind.EqualsToken);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            return syntaxNode is VariableDeclarationSyntax
                || syntaxNode is CatchDeclarationSyntax
                || syntaxNode is ForEachStatementSyntax;
        }

        protected override IEnumerable<IMisc> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var rawVariableDeclaration = syntaxNode as VariableDeclarationSyntax;
            if (rawVariableDeclaration != null)
            { return CreateFromVariableDeclaration(rawVariableDeclaration, syntaxNode, parent, model); }

            var rawCatchDeclaration = syntaxNode as CatchDeclarationSyntax;
            if (rawCatchDeclaration != null)
            {
                return new IMisc[] { GetNewVariable(VariableKind.Catch,
                  rawCatchDeclaration.Type, rawCatchDeclaration, parent, model) };
            }

            var rawForEachSyntax = syntaxNode as ForEachStatementSyntax;
            if (rawForEachSyntax != null)
            {
                return new IMisc[] { GetNewVariable(VariableKind.ForEach ,
                  rawForEachSyntax.Type, rawForEachSyntax, parent, model) };
            }

            throw new InvalidOperationException();
        }

        private IEnumerable<IMisc> CreateFromVariableDeclaration(VariableDeclarationSyntax syntax, SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<IMisc>();
            var nodeWhitespace = CreateFromWorker.GetWhitespaceSet(syntax, false);
            var declarators = syntax.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = GetNewVariable(VariableKind.Local, syntax.Type, decl, parent, model);
                var rDomItem = newItem as IRoslynDom;
                rDomItem.TokenWhitespaceSet.TokenWhitespaceList.InsertRange(0, nodeWhitespace.TokenWhitespaceList);
                list.Add(newItem);
                if (decl.Initializer != null)
                {
                    var equalsClause = decl.Initializer;
                    newItem.Initializer = Corporation.CreateFrom<IExpression>(equalsClause.Value, newItem, model).FirstOrDefault();
                    CreateFromWorker.StandardInitialize(newItem.Initializer, decl, parent, model);
                    CreateFromWorker.StoreWhitespace(newItem, decl, LanguagePart.Current, WhitespaceLookup);
                    CreateFromWorker.StoreWhitespace(newItem, decl.Initializer, LanguagePart.Current, WhitespaceLookup);
                }
            }
            return list;
        }

        public IVariable GetNewVariable(VariableKind variableKind, TypeSyntax typeSyntax,
            SyntaxNode node, IDom parent, SemanticModel model)
        {
            var newItem = new RDomVariableDeclaration(node, parent, model);
            CreateFromWorker.StandardInitialize(newItem, node, parent, model);
            newItem.Name = newItem.TypedSymbol.Name;
            var declaredType = typeSyntax.ToString();
            var returnType = Corporation
                            .CreateFrom<IMisc>(typeSyntax, newItem, model)
                            .FirstOrDefault()
                            as IReferencedType;
            newItem.Type = returnType;
            newItem.VariableKind = variableKind;

            newItem.IsImplicitlyTyped = (declaredType == "var");
            if (!newItem.IsImplicitlyTyped && declaredType != newItem.Type.Name)
            {
                var test = Mappings.AliasFromSystemType(newItem.Type.Name);
                if (declaredType == test) newItem.IsAliased = true;
            }
            return newItem;
        }


        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IVariableDeclaration;
            TypeSyntax typeSyntax;
            if (itemAsT.IsImplicitlyTyped)
            { typeSyntax = SyntaxFactory.IdentifierName("var"); }
            else
            { typeSyntax = (TypeSyntax)(RDomCSharp.Factory.BuildSyntax(itemAsT.Type)); }
            switch (itemAsT.VariableKind)
            {
                case VariableKind.Unknown:
                   throw new NotImplementedException ();
                    // This is NOT symmetric. The local declaration does not call back to this class for BuildSyntax
                //case VariableKind.Local:
                //    return BuildSyntaxLocal(itemAsT, typeSyntax);
                //case VariableKind.Using:
                //    return BuildSyntaxUsing(itemAsT, typeSyntax);
                case VariableKind.Catch:
                    return BuildSyntaxCatch(itemAsT, typeSyntax);
                //case VariableKind.For:
                //    return BuildSyntaxFor(itemAsT, typeSyntax);
                case VariableKind.ForEach:
                    return BuildSyntaxForEach (itemAsT, typeSyntax);
                default:
                    throw new InvalidOperationException();
            }
        }

        private IEnumerable<SyntaxNode> BuildSyntaxForEach(IVariableDeclaration itemAsT, TypeSyntax typeSyntax)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<SyntaxNode> BuildSyntaxCatch(IVariableDeclaration itemAsT, TypeSyntax typeSyntax)
        {
            throw new NotImplementedException();
        }

   
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomFieldTypeMemberFactory
          : RDomTypeMemberFactory<RDomField, VariableDeclaratorSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomFieldTypeMemberFactory(RDomCorporation corporation)
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
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.AccessModifiers);
                    _whitespaceLookup.AddRange(WhitespaceKindLookup.Eol);
                }
                return _whitespaceLookup;
            }
        }

        public override bool CanCreateFrom(SyntaxNode syntaxNode)
        {
            // This will conflict with Declaration statement if we don't scope factories. In that case, check parent
            return syntaxNode is FieldDeclarationSyntax;
        }

        protected override IEnumerable<ITypeMemberCommentWhite> CreateListFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var list = new List<ITypeMember>();

            var fieldPublicAnnotations = CreateFromWorker.GetPublicAnnotations(syntaxNode, parent, model);
            var rawField = syntaxNode as FieldDeclarationSyntax;
            var declarators = rawField.Declaration.Variables.OfType<VariableDeclaratorSyntax>();
            foreach (var decl in declarators)
            {
                var newItem = new RDomField(decl, parent, model);
                list.Add(newItem);
                CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
                CreateFromWorker.StoreWhitespace(newItem, syntaxNode, LanguagePart.Current, WhitespaceLookup);
                CreateFromWorker.StoreWhitespace(newItem, decl, LanguagePart.Current, WhitespaceLookup);

                newItem.Name = newItem.TypedSymbol.Name;

                if (decl.Initializer != null)
                { newItem.Initializer = Corporation.CreateFrom<IExpression>(decl.Initializer.Value, newItem, model).FirstOrDefault(); }

                //newItem.ReturnType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.Type);
                var returnType = Corporation
                                 .CreateFrom<IMisc>(rawField.Declaration.Type, newItem, model)
                                 .FirstOrDefault()
                                 as IReferencedType;
                newItem.ReturnType = returnType;

                var fieldSymbol = newItem.Symbol as IFieldSymbol;
                newItem.IsStatic = fieldSymbol.IsStatic;
                // TODO: Assign IsNew, question on insider's list
                // newItem.IsNew = fieldSymbol.i 
                newItem.IsVolatile = fieldSymbol.IsVolatile;
                newItem.IsReadOnly = fieldSymbol.IsReadOnly;
                newItem.IsConstant = fieldSymbol.HasConstantValue;
                // See note on IsNew on interface before changing
                newItem.IsNew = rawField.Modifiers.Any(x => x.CSharpKind() == SyntaxKind.NewKeyword);
                newItem.PublicAnnotations.Add(fieldPublicAnnotations);

            }
            return list;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsField = item as IField;
            var nameSyntax = SyntaxFactory.Identifier(itemAsField.Name);
            var returnTypeSyntax = (TypeSyntax)RDomCSharp.Factory.BuildSyntaxGroup(itemAsField.ReturnType).First();
            var modifiers = BuildSyntaxHelpers.BuildModfierSyntax(itemAsField);
            var declaratorNode = SyntaxFactory.VariableDeclarator(nameSyntax);
            var variableNode = SyntaxFactory.VariableDeclaration(returnTypeSyntax)
               .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(nameSyntax)));
            var node = SyntaxFactory.FieldDeclaration(variableNode)
               .WithModifiers(modifiers);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsField.Whitespace2Set, WhitespaceLookup);

            var attributes = BuildSyntaxWorker.BuildAttributeSyntax(itemAsField.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }

            //node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

            return node.PrepareForBuildSyntaxOutput(item);
        }

    }

}

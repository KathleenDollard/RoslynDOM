using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomTypeParameterFactory
           : RDomMiscFactory<RDomTypeParameter, TypeParameterSyntax>
    {
        private static WhitespaceKindLookup _whitespaceLookup;

        public RDomTypeParameterFactory(RDomCorporation corporation)
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

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as TypeParameterSyntax;
            var newItem = new RDomTypeParameter(syntax, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);
            CreateFromWorker.StoreWhitespace(newItem, syntax, LanguagePart.Current, WhitespaceLookup);

            var name = newItem.TypedSymbol.Name;
            newItem.Name = name;
            newItem.Variance = Mappings.VarianceFromVarianceKind(syntax.VarianceKeyword.CSharpKind());

            var typeParameterList = syntax.Parent.ChildNodes()
                        .OfType<TypeParameterSyntax>()
                        .Select(x => x.Identifier.ToString());
            newItem.Ordinal = typeParameterList.PreviousSiblings(name).Count();

            // parent is type parameter list. Parent parent contains constraints
            var syntaxParent = syntax.Parent.Parent;
            var constraintClauses = syntaxParent.ChildNodes()
                        .OfType<TypeParameterConstraintClauseSyntax>()
                        .ToList();
            var constraintClause = constraintClauses
                        .Where(x => x.Name.ToString() == name)
                        .FirstOrDefault();
            if (constraintClause != null)
            {
                foreach (var constraint in constraintClause.Constraints)
                {
                    var asClassStruct = constraint as ClassOrStructConstraintSyntax;
                    if (asClassStruct != null)
                    {
                        if (asClassStruct.ClassOrStructKeyword.CSharpKind()
                            == SyntaxKind.ClassKeyword)
                        { newItem.HasReferenceTypeConstraint = true; }
                        if (asClassStruct.ClassOrStructKeyword.CSharpKind()
                             == SyntaxKind.StructKeyword)
                        { newItem.HasValueTypeConstraint = true; }
                        continue;
                    }
                    var asConstructor = constraint as ConstructorConstraintSyntax;
                    if (asConstructor != null)
                    {
                        newItem.HasConstructorConstraint = true;
                        continue;
                    }
                    var asType = constraint as TypeConstraintSyntax;
                    if (asType != null)
                    {
                        var newConstraintType = Corporation
                                .CreateFrom<IMisc>(asType.Type, newItem, model)
                                .FirstOrDefault()
                                as IReferencedType;
                        if (newConstraintType != null)
                        { newItem.ConstraintTypes.AddOrMove(newConstraintType); }
                        continue;
                    }
                }

            }
            return newItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remarks>
        /// In C# syntax, type parameters and constraints are independent, so 
        /// for convenience (avoiding an extra pattern in the API) both are returned
        /// by this method. Use OfType<> to separately add them to the parent. 
        /// </remarks>
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var list = new List<SyntaxNode>();
            var itemAsT = item as ITypeParameter;
            var node = SyntaxFactory.TypeParameter(itemAsT.Name);
            node = BuildSyntaxHelpers.AttachWhitespace(node, itemAsT.Whitespace2Set, WhitespaceLookup);

            if (itemAsT.Variance != Variance.None)
            { node.WithVarianceKeyword(SyntaxFactory.Token(Mappings.VarianceKindFromVariance(itemAsT.Variance))); }
            list.Add(node);
            list.Add(GetConstraintClause(itemAsT.Name, itemAsT));

            return list.PrepareForBuildSyntaxOutput(item);
        }

        private SyntaxNode GetConstraintClause(string name, ITypeParameter itemAsT)
        {
            var list = new List<TypeParameterConstraintSyntax>();
            if (itemAsT.HasValueTypeConstraint)
            { list.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint)); }
            else if (itemAsT.HasReferenceTypeConstraint)
            { list.Add(SyntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint)); }
            if (itemAsT.HasConstructorConstraint)
            { list.Add(SyntaxFactory.ConstructorConstraint()); }

            foreach (var typeConstraint in itemAsT.ConstraintTypes)
            {
                var typeSyntax = (TypeSyntax)RDomCSharp.Factory
                                .BuildSyntax(typeConstraint);
                var typeConstraintSyntax = SyntaxFactory.TypeConstraint(typeSyntax);
                list.Add(typeConstraintSyntax);
            }
            return SyntaxFactory.TypeParameterConstraintClause(name)
                    .WithConstraints(SyntaxFactory.SeparatedList(list));
        }
    }

}

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
        public RDomTypeParameterFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IMisc CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as TypeParameterSyntax;
            var newItem = new RDomTypeParameter(syntax, parent, model);
            CreateFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            var name = newItem.TypedSymbol.Name;
            newItem.Name = name;
            newItem.Variance = Mappings.VarianceFromVarianceKind(syntax.VarianceKeyword.CSharpKind());

            var typeParameterList = syntax.Parent.ChildNodes()
                        .OfType<TypeParameterSyntax>()
                        .Select(x => x.Identifier.ToString());
            newItem.Ordinal = typeParameterList.PreviousSiblings(name).Count();

            var constraintSyntax = syntax.Parent.ChildNodes()
                        .OfType<TypeParameterConstraintClauseSyntax>()
                        .Where(x => x.Name.ToString() == name)
                        .FirstOrDefault();
            if (constraintSyntax != null)
            {
                foreach (var constraint in constraintSyntax.Constraints)
                {
                    var asClassStruct = constraint as ClassOrStructConstraintSyntax;
                    if (asClassStruct != null)
                    {
                        if (asClassStruct.ClassOrStructKeyword.CSharpKind()
                            == SyntaxKind.ClassConstraint)
                        { newItem.HasReferenceTypeConstraint = true; }
                        if (asClassStruct.ClassOrStructKeyword.CSharpKind()
                             == SyntaxKind.StructConstraint)
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
                                .CreateFrom<IMisc>(constraint, newItem, model)
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

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            var itemAsT = item as IReferencedType;
            var node = SyntaxFactory.ParseTypeName(itemAsT.Name);
            return node.PrepareForBuildSyntaxOutput(item);
        }


    }

}

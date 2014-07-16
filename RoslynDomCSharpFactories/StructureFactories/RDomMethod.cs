using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharpFactories
{
    public class RDomMethodTypeMemberFactory
          : RDomTypeMemberFactory<IMethod, MethodDeclarationSyntax>
    {
        public override void InitializeItem(IMethod newItem, MethodDeclarationSyntax rawItem)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            var typeParameters = newItem.TypedSymbol.TypeParametersFrom();
            foreach (var typeParameter in typeParameters)
            { newItem.AddTypeParameter(typeParameter); }

            newItem.AccessModifier = RoslynUtilities.GetAccessibilityFromSymbol(newItem.Symbol);
            newItem.ReturnType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.ReturnType);
            newItem.IsAbstract = newItem.Symbol.IsAbstract;
            newItem.IsVirtual = newItem.Symbol.IsVirtual;
            newItem.IsOverride = newItem.Symbol.IsOverride;
            newItem.IsSealed = newItem.Symbol.IsSealed;
            newItem.IsStatic = newItem.Symbol.IsStatic;
            newItem.IsExtensionMethod = newItem.TypedSymbol.IsExtensionMethod;
            var parameters = ListUtilities.MakeList(rawItem, x => x.ParameterList.Parameters, x => RDomFactoryHelper.MiscFactoryHelper.MakeItem(x));
            foreach (var parameter in parameters)
            { newItem.AddParameter((IParameter)parameter); }
            if (rawItem.Body != null)
            {
                var statements = ListUtilities.MakeList(rawItem, x => x.Body.Statements, x => RDomFactoryHelper.StatementFactoryHelper.MakeItem(x));
                foreach (var statement in statements)
                { newItem.AddStatement(statement); }
            }
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            var itemAsMethod = item as IMethod;
            var returnTypeSyntax = (TypeSyntax)RDomFactory.BuildSyntaxGroup(itemAsMethod.ReturnType).First();
            var modifiers = BuildSyntaxExtensions.BuildModfierSyntax(item);
            var node = SyntaxFactory.MethodDeclaration(returnTypeSyntax, nameSyntax)
                            .WithModifiers(modifiers);
            var attributes = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes); }
            var parameterSyntaxList = itemAsMethod.Parameters
                        .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
                        .OfType<ParameterSyntax>()
                        .ToList();
            if (parameterSyntaxList.Any()) { node = node.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterSyntaxList))); }
            node = node.WithBody(RoslynUtilities.MakeStatementBlock(itemAsMethod.Statements));

            // TODO: typeParameters  and constraintClauses 

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }

     }


}

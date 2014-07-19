using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomMethodTypeMemberFactory
          : RDomTypeMemberFactory<RDomMethod, MethodDeclarationSyntax>
    {
        public override IEnumerable<ITypeMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as MethodDeclarationSyntax;
            var newItem = new RDomMethod(syntaxNode,parent, model);
            newItem.Name = newItem.TypedSymbol.Name;

            var attributes = RDomFactoryHelper.GetAttributesFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

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
            var parameters = ListUtilities.MakeList(syntax, x => x.ParameterList.Parameters, x => RDomFactoryHelper.GetHelper<IMisc>().MakeItem(x, newItem, model));
            foreach (var parameter in parameters)
            { newItem.AddParameter((IParameter)parameter); }
            if (syntax.Body != null)
            {
                var statements = ListUtilities.MakeList(syntax, x => x.Body.Statements, x => RDomFactoryHelper.GetHelper<IStatement>().MakeItem(x, newItem, model));
                foreach (var statement in statements)
                { newItem.AddOrMoveStatement(statement); }
            }

            return new ITypeMember[] { newItem };
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            var itemAsMethod = item as IMethod;
            var returnTypeSyntax = (TypeSyntax)RDomCSharpFactory.Factory .BuildSyntaxGroup(itemAsMethod.ReturnType).First();
            var modifiers = BuildSyntaxExtensions.BuildModfierSyntax(item);
            var node = SyntaxFactory.MethodDeclaration(returnTypeSyntax, nameSyntax)
                            .WithModifiers(modifiers);
            var attributes = RDomFactoryHelper.BuildAttributeSyntax (item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }
            var parameterSyntaxList = itemAsMethod.Parameters
                        .SelectMany(x => RDomCSharpFactory.Factory.BuildSyntaxGroup(x))
                        .OfType<ParameterSyntax>()
                        .ToList();
            if (parameterSyntaxList.Any()) { node = node.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterSyntaxList))); }
            node = node.WithBody(RoslynCSharpUtilities.MakeStatementBlock(itemAsMethod.Statements));

            // TODO: typeParameters  and constraintClauses 

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }

     }


}

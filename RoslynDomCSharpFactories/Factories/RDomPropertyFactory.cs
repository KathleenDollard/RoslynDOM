using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public class RDomPropertyTypeMemberFactory
          : RDomTypeMemberFactory<RDomProperty, PropertyDeclarationSyntax>
    {
        protected  override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as PropertyDeclarationSyntax;
            var newItem = new RDomProperty(syntaxNode, parent, model);

            newItem.Name = newItem.TypedSymbol.Name;
            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;

            var attributes = RDomFactoryHelper.CreateAttributeFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            // TODO: Get and set accessibility
            // TODO: Type parameters and constraints
            newItem.PropertyType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.Type);
            newItem.IsAbstract = newItem.Symbol.IsAbstract;
            newItem.IsVirtual = newItem.Symbol.IsVirtual;
            newItem.IsOverride = newItem.Symbol.IsOverride;
            newItem.IsSealed = newItem.Symbol.IsSealed;
            newItem.IsStatic = newItem.Symbol.IsStatic;
            var propSymbol = newItem.Symbol as IPropertySymbol;
            if (propSymbol == null) throw new InvalidOperationException();

            newItem.CanGet = (!propSymbol.IsWriteOnly); // or check whether getAccessor is null
            newItem.CanSet = (!propSymbol.IsReadOnly); // or check whether setAccessor is null
            var getAccessorSyntax = syntax.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.GetAccessorDeclaration).FirstOrDefault();
            var setAccessorSyntax = syntax.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.SetAccessorDeclaration).FirstOrDefault();
            var accessorFactory = RDomFactoryHelper.GetHelperForMisc();
            if (accessorFactory == null) { throw new InvalidOperationException(); }
            if (getAccessorSyntax != null)
            { newItem.GetAccessor = (IAccessor)accessorFactory.MakeItems(getAccessorSyntax, newItem, model).FirstOrDefault(); }
            if (setAccessorSyntax != null)
            { newItem.SetAccessor = (IAccessor)accessorFactory.MakeItems(setAccessorSyntax, newItem, model).FirstOrDefault(); }

            return newItem ;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMemberCommentWhite item)
        {
            var itemAsProperty = item as IProperty;
            var nameSyntax = SyntaxFactory.Identifier(itemAsProperty.Name);
            var returnType = (TypeSyntax)RDomCSharpFactory.Factory.BuildSyntax(itemAsProperty.ReturnType);
            var modifiers = BuildSyntaxExtensions.BuildModfierSyntax(itemAsProperty);
            var node = SyntaxFactory.PropertyDeclaration(returnType, nameSyntax)
                            .WithModifiers(modifiers);

            var attributes = RDomFactoryHelper.BuildAttributeSyntax(itemAsProperty.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }

            var accessors = SyntaxFactory.List<AccessorDeclarationSyntax>();
            var getAccessorSyntax = RDomCSharpFactory.Factory.BuildSyntaxGroup(itemAsProperty.GetAccessor).FirstOrDefault();
            if (getAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)getAccessorSyntax); }
            var setAccessorSyntax = RDomCSharpFactory.Factory.BuildSyntaxGroup(itemAsProperty.SetAccessor).FirstOrDefault();
            if (setAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)setAccessorSyntax); }
            if (accessors.Any()) { node = node.WithAccessorList(SyntaxFactory.AccessorList(accessors)); }
            node.WithLeadingTrivia(BuildSyntaxExtensions.LeadingTrivia(item));
            // TODO: parameters , typeParameters and constraintClauses 

            // TODO: return new SyntaxNode[] { node.Format() };
            return new SyntaxNode[] { node };
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomPropertyTypeMemberFactory
          : RDomTypeMemberFactory<RDomProperty, PropertyDeclarationSyntax>
    {
        public override IEnumerable<ITypeMember> CreateFrom(SyntaxNode syntaxNode, SemanticModel model)
        {
            var syntax = syntaxNode as PropertyDeclarationSyntax;
            var newItem = new RDomProperty(syntaxNode, model);

            newItem.Name = newItem.TypedSymbol.Name;
            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;

            var attributes = RDomFactoryHelper.GetAttributesFrom(syntaxNode, newItem, model);
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
            var accessorFactory = RDomFactoryHelper.GetHelper<IMisc>();
            if (accessorFactory == null) { throw new InvalidOperationException(); }
            if (getAccessorSyntax != null)
            { newItem.GetAccessor = (IAccessor)accessorFactory.MakeItem(getAccessorSyntax, model).FirstOrDefault(); }
            if (setAccessorSyntax != null)
            { newItem.SetAccessor = (IAccessor)accessorFactory.MakeItem(setAccessorSyntax, model).FirstOrDefault(); }

            return new ITypeMember[] { newItem };
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            var itemAsProeprty = item as IProperty;
            var returnType = (TypeSyntax)RDomCSharpFactory.Factory.BuildSyntax(itemAsProeprty.ReturnType);
            var modifiers = BuildSyntaxExtensions.BuildModfierSyntax(item);
            var node = SyntaxFactory.PropertyDeclaration(returnType, nameSyntax)
                            .WithModifiers(modifiers);

            var attributes = RDomFactoryHelper.BuildAttributeSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }

            var accessors = SyntaxFactory.List<AccessorDeclarationSyntax>();
            var getAccessorSyntax = RDomCSharpFactory.Factory.BuildSyntaxGroup(itemAsProeprty.GetAccessor).FirstOrDefault();
            if (getAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)getAccessorSyntax); }
            var setAccessorSyntax = RDomCSharpFactory.Factory.BuildSyntaxGroup(itemAsProeprty.SetAccessor).FirstOrDefault();
            if (setAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)setAccessorSyntax); }
            if (accessors.Any()) { node = node.WithAccessorList(SyntaxFactory.AccessorList(accessors)); }
            // TODO: parameters , typeParameters and constraintClauses 

            return new SyntaxNode[] { node.NormalizeWhitespace() };
        }
    }

}

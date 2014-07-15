using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    internal static class RDomEnumFactoryHelper
    {
        public static void InitializeItem(IEnum newItem, EnumDeclarationSyntax syntax)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            var symbol = newItem.Symbol as INamedTypeSymbol;
            if (symbol != null)
            {
                var underlyingNamedTypeSymbol = symbol.EnumUnderlyingType;
                newItem.UnderlyingType = new RDomReferencedType(underlyingNamedTypeSymbol.DeclaringSyntaxReferences, underlyingNamedTypeSymbol);
            }
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(IEnum item)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var attributeSyntax = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            var node = SyntaxFactory.EnumDeclaration(identifier)
                .WithModifiers(modifiers);
            var itemAsEnum = item as IEnum;
            if (itemAsEnum == null) { throw new InvalidOperationException(); }
            //var membersSyntax = itemAsEnum.Members
            //            .SelectMany(x => RDomFactoryHelper.TypeMemberFactoryHelper.BuildSyntax(x))
            //            .ToList();
            //node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }

    public class RDomEnumTypeMemberFactory
        : RDomTypeMemberFactory<IEnum, EnumDeclarationSyntax>
    {
        public override void InitializeItem(IEnum newItem, EnumDeclarationSyntax syntax)
        {
            RDomEnumFactoryHelper.InitializeItem(newItem, syntax);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item);
        }
    }


    public class RDomEnumStemMemberFactory
           : RDomStemMemberFactory<IEnum, EnumDeclarationSyntax>
    {
        public override void InitializeItem(IEnum newItem, EnumDeclarationSyntax syntax)
        {
            RDomEnumFactoryHelper.InitializeItem(newItem, syntax);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item);
        }
    }


}

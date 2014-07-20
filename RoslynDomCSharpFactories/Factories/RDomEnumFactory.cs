using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom.CSharp
{
    internal static class RDomEnumFactoryHelper
    {
        public static RDomEnum CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as EnumDeclarationSyntax;
            var newItem = new RDomEnum(syntaxNode, parent, model);
            newItem.Name = newItem.TypedSymbol.Name;

            var attributes = RDomFactoryHelper.GetAttributesFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            var symbol = newItem.Symbol as INamedTypeSymbol;
            if (symbol != null)
            {
                var underlyingNamedTypeSymbol = symbol.EnumUnderlyingType;
                newItem.UnderlyingType = new RDomReferencedType(underlyingNamedTypeSymbol.DeclaringSyntaxReferences, underlyingNamedTypeSymbol);
            }

            return newItem;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(RDomEnum item)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var node = SyntaxFactory.EnumDeclaration(identifier)
                .WithModifiers(modifiers);
            var attributes = RDomFactoryHelper.BuildAttributeSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }
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
        : RDomTypeMemberFactory<RDomEnum, EnumDeclarationSyntax>
    {
        public override IEnumerable<ITypeMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomEnumFactoryHelper.CreateFrom(syntaxNode, parent, model);
            return new ITypeMember[] { ret };
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item);
        }
    }


    public class RDomEnumStemMemberFactory
           : RDomStemMemberFactory<RDomEnum, EnumDeclarationSyntax>
    {
        public override IEnumerable<IStemMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomEnumFactoryHelper.CreateFrom(syntaxNode,parent, model);
            return new IStemMember[] { ret };
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item);
        }
    }

}

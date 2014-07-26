using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom.CSharp
{
    internal static class RDomInterfaceFactoryHelper
    {
        public static RDomInterface CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as InterfaceDeclarationSyntax;
            var newItem = new RDomInterface(syntaxNode, parent,model);
            newItem.Name = newItem.TypedSymbol.Name;

            var attributes = RDomFactoryHelper.CreateAttributeFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;

            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            newItem.TypeParameters.AddOrMoveRange(newTypeParameters);

            var members = ListUtilities.MakeList(syntax, x => x.Members, x => RDomFactoryHelper.GetHelperForTypeMember().MakeItems(x, newItem, model));
            newItem.MembersAll.AddOrMoveRange(members);
            
            return newItem;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(RDomInterface item)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var node = SyntaxFactory.InterfaceDeclaration(identifier)
                .WithModifiers(modifiers);
            var attributes = RDomFactoryHelper.BuildAttributeSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }
            var itemAsInterface = item as IInterface;
            if (itemAsInterface == null) { throw new InvalidOperationException(); }
            var membersSyntax = itemAsInterface.Members
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));
            // TODO: Class type members and type constraints
            return item.PrepareForBuildSyntaxOutput(node);
        }
    }

    public class RDomInterfaceTypeMemberFactory
       : RDomTypeMemberFactory<RDomInterface, InterfaceDeclarationSyntax>
    {
        protected  override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomInterfaceFactoryHelper.CreateFrom(syntaxNode,parent, model);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMemberCommentWhite item)
        {
            return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item);
        }
    }


    public class RDomInterfaceStemMemberFactory
           : RDomStemMemberFactory<RDomInterface, InterfaceDeclarationSyntax>
    {
        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomInterfaceFactoryHelper.CreateFrom(syntaxNode, parent, model);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMemberCommentWhite item)
        {
            return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item);
        }
    }



}

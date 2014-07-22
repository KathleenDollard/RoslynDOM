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

            var attributes = RDomFactoryHelper.GetAttributesFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;

            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            newItem.TypeParameters.AddOrMoveRange(newTypeParameters);

            var members = ListUtilities.MakeList(syntax, x => x.Members, x => RDomFactoryHelper.GetHelper<ITypeMember>().MakeItem(x, newItem, model));
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
                        .SelectMany(x => RDomCSharpFactory.Factory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            node.WithLeadingTrivia(BuildSyntaxExtensions.LeadingTrivia(item));
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }

    public class RDomInterfaceTypeMemberFactory
       : RDomTypeMemberFactory<RDomInterface, InterfaceDeclarationSyntax>
    {
        public override IEnumerable<ITypeMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomInterfaceFactoryHelper.CreateFrom(syntaxNode,parent, model);
            return new ITypeMember[] { ret };
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item);
        }
    }


    public class RDomInterfaceStemMemberFactory
           : RDomStemMemberFactory<RDomInterface, InterfaceDeclarationSyntax>
    {
        public override IEnumerable<IStemMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomInterfaceFactoryHelper.CreateFrom(syntaxNode, parent,model);
            return new IStemMember[] { ret };
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item);
        }
    }



}

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    internal static class RDomInterfaceFactoryHelper
    {
        public static void InitializeItem(IInterface newItem, InterfaceDeclarationSyntax syntax)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            foreach (var typeParameter in newTypeParameters)
            { newItem.AddOrMoveTypeParameter(typeParameter); }
            var members = ListUtilities.MakeList(syntax, x => x.Members, x => RDomFactoryHelper.TypeMemberFactoryHelper.MakeItem(x));
            foreach (var member in members)
            { newItem.AddOrMoveMember(member); }
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(IInterface item)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var attributeSyntax = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            var node = SyntaxFactory.InterfaceDeclaration(identifier)
                .WithModifiers(modifiers);
            var itemAsInterface = item as IInterface;
            if (itemAsInterface == null) { throw new InvalidOperationException(); }
            var membersSyntax = itemAsInterface.Members
                        .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }

    public class RDomInterfaceTypeMemberFactory
       : RDomTypeMemberFactory<IInterface, InterfaceDeclarationSyntax>
    {
        public override void InitializeItem(IInterface newItem, InterfaceDeclarationSyntax syntax)
        {
            RDomInterfaceFactoryHelper.InitializeItem(newItem, syntax);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item);
        }
    }


    public class RDomInterfaceStemMemberFactory
           : RDomStemMemberFactory<IInterface, InterfaceDeclarationSyntax>
    {
        public override void InitializeItem(IInterface newItem, InterfaceDeclarationSyntax syntax)
        {
            RDomInterfaceFactoryHelper.InitializeItem(newItem, syntax);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            return RDomInterfaceFactoryHelper.BuildSyntax((RDomInterface)item);
        }
    }

}

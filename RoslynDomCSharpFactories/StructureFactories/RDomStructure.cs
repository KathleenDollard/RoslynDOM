using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharpFactories
{
    internal static class RDomStructureFactoryHelper
    {
        public static void InitializeItem(IStructure newItem, StructDeclarationSyntax syntax)
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

        public static IEnumerable<SyntaxNode> BuildSyntax(IStructure item)
        {
            // This is identical to Class, but didn't work out reuse here
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var attributeSyntax = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            var node = SyntaxFactory.StructDeclaration(identifier)
                .WithModifiers(modifiers);
            var itemAsStruct = item as IStructure;
            if (itemAsStruct == null) { throw new InvalidOperationException(); }
            var membersSyntax = itemAsStruct.Members
                        .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }
    public class RDomStructureTypeMemberFactory
      : RDomTypeMemberFactory<IStructure, StructDeclarationSyntax>
    {
        public override void InitializeItem(IStructure newItem, StructDeclarationSyntax syntax)
        {
            RDomStructureFactoryHelper.InitializeItem(newItem, syntax);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            return RDomStructureFactoryHelper.BuildSyntax((RDomStructure)item);
        }
    }


    public class RDomStructureStemMemberFactory
           : RDomStemMemberFactory<IStructure, StructDeclarationSyntax>
    {
        public override void InitializeItem(IStructure newItem, StructDeclarationSyntax syntax)
        {
            RDomStructureFactoryHelper.InitializeItem(newItem, syntax);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {

            return RDomStructureFactoryHelper.BuildSyntax((RDomStructure)item);
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    internal static class RDomStructureFactoryHelper
    {
        public static RDomStructure CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as StructDeclarationSyntax;
            var newItem = new RDomStructure(syntaxNode, parent,model);

            newItem.Name = newItem.TypedSymbol.Name;
            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;

            var attributes = RDomFactoryHelper.CreateAttributeFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            newItem.TypeParameters.AddOrMoveRange(newTypeParameters);

            var members = ListUtilities.MakeList(syntax, x => x.Members, x => RDomFactoryHelper.GetHelperForTypeMember().MakeItems(x, newItem, model));
            newItem.MembersAll.AddOrMoveRange(members);

            return  newItem ;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(RDomStructure item)
        {
            // This is identical to Class, but didn't work out reuse here
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var node = SyntaxFactory.StructDeclaration(identifier)
                .WithModifiers(modifiers);
            var attributes = RDomFactoryHelper.BuildAttributeSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }
            var itemAsStruct = item as IStructure;
            if (itemAsStruct == null) { throw new InvalidOperationException(); }
            var membersSyntax = itemAsStruct.Members
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));
            // TODO: Class type members and type constraints
            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
    public class RDomStructureTypeMemberFactory
      : RDomTypeMemberFactory<RDomStructure, StructDeclarationSyntax>
    {
        protected  override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomStructureFactoryHelper.CreateFrom(syntaxNode, parent, model);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMemberCommentWhite item)
        {
            return RDomStructureFactoryHelper.BuildSyntax((RDomStructure)item);
        }
    }

    public class RDomStructureStemMemberFactory
           : RDomStemMemberFactory<RDomStructure, StructDeclarationSyntax>
    {
        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomStructureFactoryHelper.CreateFrom(syntaxNode, parent, model);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMemberCommentWhite item)
        {

            return RDomStructureFactoryHelper.BuildSyntax((RDomStructure)item);
        }
    }
     
}

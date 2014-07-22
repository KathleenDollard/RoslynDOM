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

            var attributes = RDomFactoryHelper.GetAttributesFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            foreach (var typeParameter in newTypeParameters)
            { newItem.AddOrMoveTypeParameter(typeParameter); }

            var members = ListUtilities.MakeList(syntax, x => x.Members, x => RDomFactoryHelper.GetHelper<ITypeMember>().MakeItem(x, newItem, model));
            foreach (var member in members)
            { newItem.AddOrMoveMember(member); }

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
                        .SelectMany(x => RDomCSharpFactory.Factory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            node.WithLeadingTrivia(BuildSyntaxExtensions.LeadingTrivia(item));
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }
    public class RDomStructureTypeMemberFactory
      : RDomTypeMemberFactory<RDomStructure, StructDeclarationSyntax>
    {
        public override IEnumerable<ITypeMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomStructureFactoryHelper.CreateFrom(syntaxNode, parent, model);
            return new ITypeMember[] { ret };
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            return RDomStructureFactoryHelper.BuildSyntax((RDomStructure)item);
        }
    }

    public class RDomStructureStemMemberFactory
           : RDomStemMemberFactory<RDomStructure, StructDeclarationSyntax>
    {
        public override IEnumerable<IStemMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomStructureFactoryHelper.CreateFrom(syntaxNode, parent,model);
            return new IStemMember[] { ret };
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {

            return RDomStructureFactoryHelper.BuildSyntax((RDomStructure)item);
        }
    }
     
}

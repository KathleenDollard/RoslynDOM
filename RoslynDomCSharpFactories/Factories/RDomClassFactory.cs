using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    internal static class RDomClassFactoryHelper
    {
        internal static RDomClass CreateFromInternal(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ClassDeclarationSyntax;
            var newItem = new RDomClass(syntaxNode, parent, model);
            newItem.Name = newItem.TypedSymbol.Name;

            var attributes = RDomFactoryHelper.CreateAttributeFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            newItem.TypeParameters.AddOrMoveRange(newTypeParameters);
            var members = ListUtilities.MakeList(syntax, x => x.Members, x => RDomFactoryHelper.GetHelperForTypeMember().MakeItems(x, newItem, model));
            newItem.MembersAll.AddOrMoveRange(members);
            newItem.BaseType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.BaseType);
            newItem.IsAbstract = newItem.Symbol.IsAbstract;
            newItem.IsSealed = newItem.Symbol.IsSealed;
            newItem.IsStatic = newItem.Symbol.IsStatic;

            return newItem;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(RDomClass item)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var node = SyntaxFactory.ClassDeclaration(identifier)
                .WithModifiers(modifiers);
            var itemAsClass = item as IClass;
            if (itemAsClass == null) { throw new InvalidOperationException(); }
            var attributes = RDomFactoryHelper.BuildAttributeSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }

            var membersSyntax = itemAsClass.Members
                        .SelectMany(x => RDomCSharpFactory.Factory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));

            // TODO: Class type members and type constraints

            return item.PrepareForBuildSyntaxOutput(node);
        }
    }
    public class RDomClassTypeMemberFactory
           : RDomTypeMemberFactory<RDomClass, ClassDeclarationSyntax>
    {
        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomClassFactoryHelper.CreateFromInternal(syntaxNode, parent, model);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMemberCommentWhite item)
        {
            return RDomClassFactoryHelper.BuildSyntax((RDomClass)item);
        }
    }

    public class RDomClassStemMemberFactory
           : RDomStemMemberFactory<RDomClass, ClassDeclarationSyntax>
    {
        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomClassFactoryHelper.CreateFromInternal(syntaxNode, parent, model);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMemberCommentWhite item)
        {
            return RDomClassFactoryHelper.BuildSyntax((RDomClass)item);
        }
    }

}

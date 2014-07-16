using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    internal static class RDomClassFactoryHelper
    {
        public static void InitializeItem(RDomClass newItem, ClassDeclarationSyntax syntax)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            foreach (var typeParameter in newTypeParameters)
            { newItem.AddOrMoveTypeParameter(typeParameter); }
            var members = ListUtilities.MakeList(newItem.TypedSyntax, x => x.Members, x => RDomFactoryHelper.GetHelper<ITypeMember>().MakeItem(x));
            foreach (var member in members)
            { newItem.AddOrMoveMember(member); }
            newItem.BaseType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.BaseType);
            newItem.IsAbstract = newItem.Symbol.IsAbstract;
            newItem.IsSealed = newItem.Symbol.IsSealed;
            newItem.IsStatic = newItem.Symbol.IsStatic;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(RDomClass item)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var node = SyntaxFactory.ClassDeclaration(identifier)
                .WithModifiers(modifiers);
            var itemAsClass = item as IClass;
            if (itemAsClass == null) { throw new InvalidOperationException(); }
            var attributes = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes); }
            var membersSyntax = itemAsClass.Members
                        .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }
    public class RDomClassTypeMemberFactory
           : RDomTypeMemberFactory<RDomClass, ClassDeclarationSyntax>
    {
        public override void InitializeItem(RDomClass newItem, ClassDeclarationSyntax syntax)
        {
            RDomClassFactoryHelper.InitializeItem(newItem, syntax);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            return RDomClassFactoryHelper.BuildSyntax((RDomClass)item);
        }
    }

    public class RDomClassStemMemberFactory
           : RDomStemMemberFactory<RDomClass, ClassDeclarationSyntax>
    {
        public override void InitializeItem(RDomClass newItem, ClassDeclarationSyntax syntax)
        {
            RDomClassFactoryHelper.InitializeItem(newItem, syntax);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            return RDomClassFactoryHelper.BuildSyntax((RDomClass)item);
        }
    }

}

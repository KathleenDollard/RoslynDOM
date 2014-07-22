using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    public static class RDomClassFactoryHelper
    {
        internal static RDomClass CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var syntax = syntaxNode as ClassDeclarationSyntax;
            var newItem = new RDomClass(syntaxNode,parent, model);
                        newItem.Name = newItem.TypedSymbol.Name;

            var attributes = RDomFactoryHelper.GetAttributesFrom(syntaxNode, newItem, model);
            newItem.Attributes.AddOrMoveAttributeRange(attributes);

            newItem.AccessModifier = (AccessModifier)newItem.Symbol.DeclaredAccessibility;
            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            newItem.TypeParameters.AddOrMoveRange(newTypeParameters);
                  var members = ListUtilities.MakeList(syntax, x => x.Members, x => RDomFactoryHelper.GetHelper<ITypeMember>().MakeItem(x, newItem, model));
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

            node.WithLeadingTrivia(BuildSyntaxExtensions.LeadingTrivia(item));
            
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }
    public class RDomClassTypeMemberFactory
           : RDomTypeMemberFactory<RDomClass, ClassDeclarationSyntax>
    {
        public override IEnumerable<ITypeMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomClassFactoryHelper.CreateFrom(syntaxNode,parent, model);
            return new ITypeMember[] { ret };
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            return RDomClassFactoryHelper.BuildSyntax((RDomClass)item);
        }
    }

    public class RDomClassStemMemberFactory
           : RDomStemMemberFactory<RDomClass, ClassDeclarationSyntax>
    {
        public override IEnumerable<IStemMember> CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomClassFactoryHelper.CreateFrom(syntaxNode,parent, model);
            return new IStemMember[] { ret };
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            return RDomClassFactoryHelper.BuildSyntax((RDomClass)item);
        }
    }

}

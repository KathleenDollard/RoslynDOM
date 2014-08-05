using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    internal static class RDomClassFactoryHelper
    {
        [ExcludeFromCodeCoverage]
        // until move to C# 6 - I want to support name of as soon as possible
        private static string nameof<T>(T value) { return ""; }

        internal static RDomClass CreateFromInternal(SyntaxNode syntaxNode, IDom parent, SemanticModel model,
               ICreateFromWorker  createFromWorker, RDomCorporation corporation)
        {
            var syntax = syntaxNode as ClassDeclarationSyntax;
            var newItem = new RDomClass(syntaxNode, parent, model);
            createFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            newItem.Name = newItem.TypedSymbol.Name;

            var newTypeParameters = newItem.TypedSymbol.TypeParametersFrom();
            newItem.TypeParameters.AddOrMoveRange(newTypeParameters);
            var members = ListUtilities.MakeList(syntax, x => x.Members, x => corporation.CreateFrom<ITypeMemberCommentWhite>(x, newItem, model));
            newItem.MembersAll.AddOrMoveRange(members);
            newItem.BaseType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.BaseType);
            newItem.IsAbstract = newItem.Symbol.IsAbstract;
            newItem.IsSealed = newItem.Symbol.IsSealed;
            newItem.IsStatic = newItem.Symbol.IsStatic;

            return newItem;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(RDomClass item, 
            ICSharpBuildSyntaxWorker buildSyntaxWorker, RDomCorporation corporation)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var node = SyntaxFactory.ClassDeclaration(identifier)
                .WithModifiers(modifiers);
            var itemAsClass = item as IClass;
            Guardian.Assert.IsNotNull(itemAsClass, nameof(itemAsClass));
            var attributes = buildSyntaxWorker.BuildAttributeSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes.WrapInAttributeList()); }

            var membersSyntax = itemAsClass.Members
                        .SelectMany(x => RDomCSharp.Factory.BuildSyntaxGroup(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));

            // TODO: Class type members and type constraints

            return node.PrepareForBuildSyntaxOutput(item);
        }
    }
    public class RDomClassTypeMemberFactory
           : RDomTypeMemberFactory<RDomClass, ClassDeclarationSyntax>
    {
        public RDomClassTypeMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomClassFactoryHelper.CreateFromInternal(syntaxNode, parent, model, CreateFromWorker, Corporation);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            return RDomClassFactoryHelper.BuildSyntax((RDomClass)item, BuildSyntaxWorker, Corporation);
        }
    }

    public class RDomClassStemMemberFactory
           : RDomStemMemberFactory<RDomClass, ClassDeclarationSyntax>
    {
        public RDomClassStemMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomClassFactoryHelper.CreateFromInternal(syntaxNode, parent, model, CreateFromWorker, Corporation);
        }
        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            return RDomClassFactoryHelper.BuildSyntax((RDomClass)item, BuildSyntaxWorker, Corporation );
        }
    }

}

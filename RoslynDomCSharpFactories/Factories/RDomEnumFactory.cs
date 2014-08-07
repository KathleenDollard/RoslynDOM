using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace RoslynDom.CSharp
{
    internal static class RDomEnumFactoryHelper
    {
        // until move to C# 6 - I want to support name of as soon as possible
        [ExcludeFromCodeCoverage]
        private static string nameof<T>(T value) { return ""; }

        public static RDomEnum CreateFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model, ICreateFromWorker createFromWorker, RDomCorporation corporation)
        {
            var syntax = syntaxNode as EnumDeclarationSyntax;
            var newItem = new RDomEnum(syntaxNode, parent, model);
            createFromWorker.StandardInitialize(newItem, syntaxNode, parent, model);

            newItem.Name = newItem.TypedSymbol.Name;

            var symbol = newItem.Symbol as INamedTypeSymbol;
            if (symbol != null)
            {
                var underlyingNamedTypeSymbol = symbol.EnumUnderlyingType;
                // TODO: underlying type should be set to Int when there is not type specified,there is always an underlying type
                if (syntax.BaseList != null)
                {
                    //newItem.UnderlyingType = new RDomReferencedType(underlyingNamedTypeSymbol.DeclaringSyntaxReferences, underlyingNamedTypeSymbol);
                    var type = corporation
                                    .CreateFrom<IMisc>(syntax.BaseList.Types.First(), newItem, model)
                                    .FirstOrDefault()
                                    as IReferencedType;
                    newItem.UnderlyingType = type;
                }
            }

            foreach (var member in syntax.Members)
            {
                var newEnumValue = new RDomEnumValue(member, newItem, model);
                createFromWorker.StandardInitialize(newEnumValue, member, newItem, model);
                newEnumValue.Name = member.Identifier.ToString();
                if (member.EqualsValue != null)
                {
                    newEnumValue.Expression = corporation.CreateFrom<IExpression>(member.EqualsValue.Value, newItem, model).FirstOrDefault();
                }
                newItem.Values.AddOrMove(newEnumValue);
            }

            return newItem;
        }

        public static IEnumerable<SyntaxNode> BuildSyntax(RDomEnum item, ICSharpBuildSyntaxWorker buildSyntaxWorker, RDomCorporation corporation)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var node = SyntaxFactory.EnumDeclaration(identifier)
                .WithModifiers(modifiers);
            var attributes = buildSyntaxWorker.BuildAttributeSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(BuildSyntaxHelpers.WrapInAttributeList(attributes)); }
            var itemAsEnum = item as IEnum;
            Guardian.Assert.IsNotNull(itemAsEnum, nameof(itemAsEnum));

            node = node.WithLeadingTrivia(BuildSyntaxHelpers.LeadingTrivia(item));

            return node.PrepareForBuildSyntaxOutput(item);
        }
    }

    public class RDomEnumTypeMemberFactory
        : RDomTypeMemberFactory<RDomEnum, EnumDeclarationSyntax>
    {
        public RDomEnumTypeMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override ITypeMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            var ret = RDomEnumFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
            return ret;
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item, BuildSyntaxWorker, Corporation);
        }
    }


    public class RDomEnumStemMemberFactory
           : RDomStemMemberFactory<RDomEnum, EnumDeclarationSyntax>
    {
        public RDomEnumStemMemberFactory(RDomCorporation corporation)
            : base(corporation)
        { }

        protected override IStemMemberCommentWhite CreateItemFrom(SyntaxNode syntaxNode, IDom parent, SemanticModel model)
        {
            return RDomEnumFactoryHelper.CreateFrom(syntaxNode, parent, model, CreateFromWorker, Corporation);
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(IDom item)
        {
            return RDomEnumFactoryHelper.BuildSyntax((RDomEnum)item, BuildSyntaxWorker, Corporation);
        }
    }

}

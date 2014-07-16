using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttribute : RDomBase<IAttribute, ISymbol>, IAttribute
    {
        private IEnumerable<IAttributeValue> _attributeValues;

        internal RDomAttribute(
            AttributeSyntax rawItem,
            IEnumerable<IAttributeValue> attributeValues)
            : base(rawItem)
        {
            _attributeValues = attributeValues;
            Initialize();
        }

        internal RDomAttribute(
            RDomAttribute oldRDom)
            : base(oldRDom)
        {
            var newAttributeValues = RoslynDomUtilities.CopyMembers(oldRDom._attributeValues);
            _attributeValues = newAttributeValues;
        }

        protected override void Initialize()
        {
            Name = ((AttributeSyntax)TypedSyntax).Name.ToString();
        }

        public  AttributeSyntax BuildSyntax()
        {
            var arguments = new SeparatedSyntaxList<AttributeArgumentSyntax>();
            arguments = arguments.AddRange(_attributeValues.Select(x =>((RDomAttributeValue)x).BuildSyntax()));
            var argumentList = SyntaxFactory.AttributeArgumentList(arguments);
            var nameSyntax = SyntaxFactory.ParseName(Name);
            return SyntaxFactory.Attribute(nameSyntax, argumentList);
        }

        internal static IEnumerable<IAttribute> MakeAttributes(
            RDomBase attributedItem, ISymbol symbol, SyntaxNode syntax)
        {
            var ret = new List<IAttribute>();
            var symbolAttributes = symbol.GetAttributes();
            foreach (var attributeData in symbolAttributes)
            {
                // TODO: In those cases where we do have a symbol reference to the attribute, try to use it
                var appRef = attributeData.ApplicationSyntaxReference;
                var attribSyntax = syntax.FindNode(appRef.Span) as AttributeSyntax;
                var values = RDomAttributeValue.MakeAttributeValues(attribSyntax, attributedItem);
                var newAttribute = new RDomAttribute(attribSyntax, values);
                ret.Add(newAttribute);
            }
            return ret;
        }

        public IEnumerable<IAttributeValue> AttributeValues
        {
            get
            { return _attributeValues; }
        }
        
    }
}

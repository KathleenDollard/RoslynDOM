using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomAttribute : RDomBase<IAttribute, ISymbol>, IAttribute
    {
        private List<IAttributeValue> _attributeValues = new List < IAttributeValue > ();

        public RDomAttribute(
            SyntaxNode rawItem,
            SemanticModel model)
            : base(rawItem, model)
        {}

        internal RDomAttribute(
            RDomAttribute oldRDom)
            : base(oldRDom)
        {
            var newAttributeValues = RoslynDomUtilities.CopyMembers(oldRDom._attributeValues);
            foreach(var value in newAttributeValues )
            { AddOrMoveAttributeValue (value); }
        }

        //protected override void Initialize()
        //{
        //    Name = ((AttributeSyntax)TypedSyntax).Name.ToString();
        //}

        //public AttributeSyntax BuildSyntax()
        //{
        //    var arguments = new SeparatedSyntaxList<AttributeArgumentSyntax>();
        //    arguments = arguments.AddRange(_attributeValues.Select(x => ((RDomAttributeValue)x).BuildSyntax()));
        //    var argumentList = SyntaxFactory.AttributeArgumentList(arguments);
        //    var nameSyntax = SyntaxFactory.ParseName(Name);
        //    return SyntaxFactory.Attribute(nameSyntax, argumentList);
        //}

        //internal static IEnumerable<IAttribute> MakeAttributes(
        //    RDomBase attributedItem, ISymbol symbol, SyntaxNode syntax, SemanticModel model)
        //{
        //    var ret = new List<IAttribute>();
        //    var symbolAttributes = symbol.GetAttributes();
        //    foreach (var attributeData in symbolAttributes)
        //    {
        //        // TODO: In those cases where we do have a symbol reference to the attribute, try to use it
        //        var appRef = attributeData.ApplicationSyntaxReference;
        //        var attribSyntax = syntax.SyntaxTree.GetRoot().FindNode(appRef.Span) as AttributeSyntax;
        //        //var attribSyntax = syntax.FindNode(appRef.Span) as AttributeSyntax;
        //        var values = RDomAttributeValue.MakeAttributeValues(attribSyntax, attributedItem, model);
        //        var newAttribute = new RDomAttribute(attribSyntax, values, model);
        //        ret.Add(newAttribute);
        //    }
        //    return ret;
        //}

        public void RemoveAttributeValue(IAttributeValue attributeValue)
        { _attributeValues.Remove(attributeValue); }

        public void AddOrMoveAttributeValue(IAttributeValue attributeValue)
        { _attributeValues.Add(attributeValue); }

        public IEnumerable<IAttributeValue> AttributeValues
        { get { return _attributeValues; } }

    }
}

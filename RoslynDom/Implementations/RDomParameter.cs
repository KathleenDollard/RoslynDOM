using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    public class RDomParameterMiscFactory
            : RDomMiscFactory<RDomParameter, ParameterSyntax>
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IMisc item)
        {
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            var itemAsT = item as IParameter;
            var syntaxType = (TypeSyntax)(RDomFactory.BuildSyntax(itemAsT.Type));

            var node = SyntaxFactory.Parameter(nameSyntax)
                        .WithType(syntaxType);

            var attributes = BuildSyntaxExtensions.BuildAttributeListSyntax(itemAsT.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes); }

            var modifiers = SyntaxFactory.TokenList();
            if (itemAsT.IsOut) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.OutKeyword)); }
            if (itemAsT.IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
            if (itemAsT.IsParamArray) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ParamsKeyword)); }
            if (itemAsT.IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
            if (modifiers.Any()) { node = node.WithModifiers(modifiers); }
            return new SyntaxNode[] { node.NormalizeWhitespace() };

        }
    }


    public class RDomParameter : RDomBase<IParameter, ParameterSyntax, IParameterSymbol>, IParameter
    {
        private IReferencedType _type;
        private bool _isOut;
        private bool _isRef;
        private bool _isParamArray;
        private bool _isOptional;
        private int _ordinal;

        internal RDomParameter(
             ParameterSyntax rawItem)
           : base(rawItem)
        {
            Initialize2();
        }

        internal RDomParameter(
            ParameterSyntax rawItem,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            Initialize();
        }

        internal RDomParameter(RDomParameter oldRDom)
             : base(oldRDom)
        {
            _type = oldRDom._type;
            _isOut = oldRDom._isOut;
            _isRef = oldRDom._isRef;
            _isParamArray = oldRDom._isParamArray;
            _isOptional = oldRDom._isOptional;
            _ordinal = oldRDom._ordinal;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _type = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type);
            _isOut = TypedSymbol.RefKind == RefKind.Out;
            _isRef = TypedSymbol.RefKind == RefKind.Ref;
            _isParamArray = TypedSymbol.IsParams;
            _isOptional = TypedSymbol.IsOptional;
            _ordinal = TypedSymbol.Ordinal;
        }

        private void Initialize2()
        { Initialize();  }

        public override ParameterSyntax BuildSyntax()
        {
            var identifier = SyntaxFactory.Identifier(Name);
            var syntaxType = ((RDomReferencedType)Type).BuildSyntax();
            var node = SyntaxFactory.Parameter(identifier)
                        .WithType(syntaxType);
            var attributes = BuildAttributeListSyntax();
            if ( attributes.Any())
            { node = node.WithAttributeLists(attributes); }
            var modifiers = SyntaxFactory.TokenList();
            if (IsOut) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.OutKeyword)); }
            if (IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
            if (IsParamArray) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ParamsKeyword )); }
            if (IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
            if (modifiers.Any()) { node = node.WithModifiers(modifiers); }
            return node;
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public IReferencedType Type
        { get { return _type; } }

        public bool IsOut
        { get { return _isOut; } }

        public bool IsRef
        { get { return _isRef; } }

        public bool IsParamArray
        { get { return _isParamArray; } }

        public bool IsOptional
        { get { return _isOptional; } }

        public int Ordinal
        { get { return _ordinal; } }

        // TODO: Default Values for parameters!!!

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            {
                return Type.QualifiedName;
            }
            return base.RequestValue(name);
        }
    }


}

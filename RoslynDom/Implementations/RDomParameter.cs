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
        public override void InitializeItem(RDomParameter newItem, ParameterSyntax syntax)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            newItem.Type = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.Type);
            newItem.IsOut = newItem.TypedSymbol.RefKind == RefKind.Out;
            newItem.IsRef = newItem.TypedSymbol.RefKind == RefKind.Ref;
            newItem.IsParamArray = newItem.TypedSymbol.IsParams;
            newItem.IsOptional = newItem.TypedSymbol.IsOptional;
            newItem.Ordinal = newItem.TypedSymbol.Ordinal;
        }

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
     

        internal RDomParameter(
             ParameterSyntax rawItem)
           : base(rawItem)
        {
            //Initialize2();
        }

        //internal RDomParameter(
        //    ParameterSyntax rawItem,
        //    params PublicAnnotation[] publicAnnotations)
        //  : base(rawItem, publicAnnotations)
        //{
        //    Initialize();
        //}

        internal RDomParameter(RDomParameter oldRDom)
             : base(oldRDom)
        {
            Type = oldRDom.Type;
            IsOut = oldRDom.IsOut;
            IsRef = oldRDom.IsRef;
            IsParamArray = oldRDom.IsParamArray;
            IsOptional = oldRDom.IsOptional;
            Ordinal = oldRDom.Ordinal;
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //    _type = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type);
        //    _isOut = TypedSymbol.RefKind == RefKind.Out;
        //    _isRef = TypedSymbol.RefKind == RefKind.Ref;
        //    _isParamArray = TypedSymbol.IsParams;
        //    _isOptional = TypedSymbol.IsOptional;
        //    _ordinal = TypedSymbol.Ordinal;
        //}

        //private void Initialize2()
        //{ Initialize();  }

        //public override ParameterSyntax BuildSyntax()
        //{
        //    var identifier = SyntaxFactory.Identifier(Name);
        //    var syntaxType = ((RDomReferencedType)Type).BuildSyntax();
        //    var node = SyntaxFactory.Parameter(identifier)
        //                .WithType(syntaxType);
        //    var attributes = BuildAttributeListSyntax();
        //    if ( attributes.Any())
        //    { node = node.WithAttributeLists(attributes); }
        //    var modifiers = SyntaxFactory.TokenList();
        //    if (IsOut) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.OutKeyword)); }
        //    if (IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
        //    if (IsParamArray) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ParamsKeyword )); }
        //    if (IsRef) { modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.RefKeyword)); }
        //    if (modifiers.Any()) { node = node.WithModifiers(modifiers); }
        //    return node;
        //}

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public IReferencedType Type { get; set; }

        public bool IsOut { get; set; }

        public bool IsRef { get; set; }

        public bool IsParamArray { get; set; }

        public bool IsOptional { get; set; }

        public int Ordinal { get; set; }

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

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomParameter : RDomBase<IParameter, ParameterSyntax, IParameterSymbol>, IParameter
    {
        private IReferencedType _type;
        private bool _isOut;
        private bool _isRef;
        private bool _isParamArray;
        private bool _isOptional;
        private int _ordinal;

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

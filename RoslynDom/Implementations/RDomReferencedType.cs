using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{

    public class RDomReferencedType : RDomBase<IReferencedType, ISymbol>, IReferencedType
    {

        public RDomReferencedType(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem, parent, model)
        { Initialize(); }


        //public RDomReferencedType(ImmutableArray<SyntaxReference> raw, ISymbol symbol)
        //{
        //    _raw = raw;
        //    _symbol = symbol;
        //    Initialize();
        //}

        //public RDomReferencedType(TypeInfo typeInfo, ISymbol symbol)
        //{
        //    _typeInfo = typeInfo;
        //    _symbol = symbol == null ? typeInfo.Type : symbol;
        //    Initialize();
        //}

        public RDomReferencedType(RDomReferencedType oldRDom)
             : base(oldRDom)
        {
            Name = oldRDom.Name;
            Namespace = oldRDom.Namespace;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        public string Name { get; set; }

        public string OuterName
        { get { return Name; } }

        public string QualifiedName
        { get { return Name; } }

        public string Namespace { get; set; }


    }
}

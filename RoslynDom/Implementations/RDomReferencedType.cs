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
        public bool DisplayAlias { get; set; }
         public bool IsArray { get; set; }

        public string OuterName
        {
            get
            {
                var containingType = (string.IsNullOrEmpty(ContainingTypeName))
                                            ? ""
                                            : ContainingTypeName + ".";
                 return containingType + Name;
            }
        }

        public string QualifiedName
        {
            get
            {
                var containingType = (string.IsNullOrEmpty(ContainingTypeName))
                                            ? ""
                                            : ContainingTypeName + ".";
                var ns = (string.IsNullOrEmpty(Namespace))
                            ? ""
                            : Namespace + ".";
                return ns + containingType + Name;
            }
        }

        public string Namespace { get; set; }

        public string ContainingTypeName { get; set; }
    }
}

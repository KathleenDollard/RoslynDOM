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
        { }

        internal RDomReferencedType(RDomReferencedType oldRDom)
            : base(oldRDom)
        {
            Name = oldRDom.Name;
            Namespace = oldRDom.Namespace;
            DisplayAlias = oldRDom.DisplayAlias;
            IsArray = oldRDom.IsArray;
            ContainingType = oldRDom.ContainingType;
        }

        public string Name { get; set; }
        public bool DisplayAlias { get; set; }
        public bool IsArray { get; set; }

        public string QualifiedName
        {
            get
            {
                var containingTypename = (ContainingType == null)
                                            ? ""
                                            : ContainingType.Name + ".";
                var ns = (string.IsNullOrEmpty(Namespace))
                            ? ""
                            : Namespace + ".";
                return ns + containingTypename + Name;
            }
        }

        public string Namespace { get; set; }

        public INamedTypeSymbol ContainingType { get; set; }
    }
}

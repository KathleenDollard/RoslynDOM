using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStructure : RDomBaseType<IStructure>, IStructure
    {
        private AttributeList _attributes = new AttributeList();

        public RDomStructure(SyntaxNode rawItem, IDom parent, SemanticModel model)
           : base(rawItem,parent,  model, MemberKind.Structure, StemMemberKind.Structure)
        { }

        internal RDomStructure(RDomStructure oldRDom)
            : base(oldRDom)
        { }

        public AttributeList Attributes
        { get { return _attributes; } }

        public IEnumerable<IClass> Classes
        {
            get
            { return Members.OfType<IClass>(); }
        }

        public IEnumerable<IType> Types
        {
            get
            {
                IEnumerable<IType> ret = Classes.Concat<IType>(Structures).Concat(Interfaces).Concat(Enums);
                return ret;
            }
        }

        public IEnumerable<IStructure> Structures
        {
            get
            { return Members.OfType<IStructure>(); }
        }

        public IEnumerable<IInterface> Interfaces
        {
            get
            { return Members.OfType<IInterface>(); }
        }

        public IEnumerable<IEnum> Enums
        {
            get
            { return Members.OfType<IEnum>(); }
        }

        public IEnumerable<ITypeParameter> TypeParameters
        {
            get
            {
                return this.TypedSymbol.TypeParametersFrom();
            }
        }

        public IEnumerable<IReferencedType> ImplementedInterfaces
        {
            get
            {
                return this.ImpementedInterfacesFrom(false);
            }
        }

        public IEnumerable<IReferencedType> AllImplementedInterfaces
        {
            get
            {
                return this.ImpementedInterfacesFrom(true);
            }
        }
    }
}

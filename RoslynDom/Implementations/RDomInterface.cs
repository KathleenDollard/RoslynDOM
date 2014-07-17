using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInterface : RDomBaseType<IInterface>, IInterface
    {
        public RDomInterface(SyntaxNode rawItem, SemanticModel model)
        : base(rawItem, model, MemberKind.Interface, StemMemberKind.Interface)
        { }

        internal RDomInterface(RDomInterface oldRDom)
             : base(oldRDom)
        { }

        public IEnumerable<IReferencedType> AllImplementedInterfaces
        {
            get
            {
                return this.ImpementedInterfacesFrom(true);
            }
        }

        public IEnumerable<IReferencedType> ImplementedInterfaces
        {
            get
            {
                return this.ImpementedInterfacesFrom(false);
            }
        }

        public IEnumerable<ITypeParameter> TypeParameters
        {
            get
            {
                return this.TypedSymbol.TypeParametersFrom();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInterface : RDomBaseType<IInterface,InterfaceDeclarationSyntax>, IInterface
    {
        internal RDomInterface(
            InterfaceDeclarationSyntax rawItem,
            IEnumerable<ITypeMember> members,
            params PublicAnnotation[] publicAnnotations) 
            : base(rawItem, MemberType.Interface,StemMemberType.Interface,  members, publicAnnotations )
        {
            Initialize();
        }

        internal RDomInterface(RDomInterface oldRDom)
             : base(oldRDom)
        { }

        public override bool SameIntent(IInterface  other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (!CheckSameIntentChildList(TypeParameters, other.TypeParameters)) return false;
            if (!CheckSameIntentChildList(AllImplementedInterfaces, other.AllImplementedInterfaces)) return false;
            return true;
        }

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

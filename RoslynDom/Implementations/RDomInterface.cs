using System.Collections.Generic;
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
            : base(rawItem, MemberKind.Interface,StemMemberKind.Interface,  members, publicAnnotations )
        {
            Initialize();
        }

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

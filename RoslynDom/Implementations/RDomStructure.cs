using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStructure : RDomBaseType<IStructure, StructDeclarationSyntax>, IStructure
    {
        internal RDomStructure(
            StructDeclarationSyntax rawItem,
            IEnumerable<ITypeMember> members,
            params PublicAnnotation[] publicAnnotations)
            : base(rawItem, MemberKind.Structure, StemMemberKind.Structure, members, publicAnnotations)
        {
            Initialize();
        }
        internal RDomStructure(RDomStructure oldRDom)
             : base(oldRDom)
        { }

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

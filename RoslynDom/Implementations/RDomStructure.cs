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
            : base(rawItem, MemberType.Structure, StemMemberType.Structure, members, publicAnnotations)
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

        protected override bool CheckSameIntent(IStructure other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
            if (!CheckSameIntentChildList(Classes, other.Classes)) return false;
            if (!CheckSameIntentChildList(Structures, other.Structures)) return false;
            if (!CheckSameIntentChildList(Interfaces, other.Interfaces)) return false;
            if (!CheckSameIntentChildList(Enums, other.Enums)) return false;
            if (!CheckSameIntentChildList(TypeParameters, other.TypeParameters)) return false;
            if (!CheckSameIntentChildList(AllImplementedInterfaces, other.AllImplementedInterfaces)) return false;
            return true;
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

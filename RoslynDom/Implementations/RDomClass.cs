using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// I'm not currently supporting parameters (am supporting type parameters) because I don't understand them
    /// </remarks>
    public class RDomClass : RDomBaseType<IClass, ClassDeclarationSyntax>, IClass
    {
        internal RDomClass(ClassDeclarationSyntax rawItem,
            IEnumerable<ITypeMember> members,
            params PublicAnnotation[] publicAnnotations)
            : base(rawItem, MemberType.Class, StemMemberType.Class, members, publicAnnotations)
        {
            Initialize();
        }

        internal RDomClass(RDomClass oldRDom)
             : base(oldRDom)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsAbstract = Symbol.IsAbstract;
            IsSealed = Symbol.IsSealed;
            IsStatic = Symbol.IsStatic;
        }

        protected override bool CheckSameIntent(IClass other, bool includePublicAnnotations)
        {
            if (other == null) return false;
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
            if (IsAbstract != other.IsAbstract) return false;
            if (IsSealed != other.IsSealed) return false;
            if (IsStatic != other.IsStatic) return false;
            if (!BaseType.SameIntent(other.BaseType)) return false;
            if (!CheckSameIntentChildList(Classes, other.Classes)) return false;
            if (!CheckSameIntentChildList(Structures, other.Structures)) return false;
            if (!CheckSameIntentChildList(Interfaces, other.Interfaces)) return false;
            if (!CheckSameIntentChildList(Enums, other.Enums)) return false;
            if (!CheckSameIntentChildList(TypeParameters, other.TypeParameters)) return false;
            if (!CheckSameIntentChildList(AllImplementedInterfaces, other.AllImplementedInterfaces)) return false;
            return true;
        }

        public IEnumerable<IClass> Classes
        { get { return Members.OfType<IClass>(); } }

        public IEnumerable<IStemMember> Types
        //{ get { return Classes.Concat<IStemMember>(Structures).Concat(Interfaces).Concat(Enums); } }
        { get { return Members.OfType<IType>(); } }


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

        public bool IsAbstract { get; set; }

        public bool IsSealed { get; set; }

        public bool IsStatic { get; set; }


        public IEnumerable<ITypeParameter> TypeParameters
        {
            get
            {
                return this.TypedSymbol.TypeParametersFrom();
            }
        }

        public IReferencedType BaseType
        {
            get
            {
                return new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.BaseType);
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
        { get { return this.ImpementedInterfacesFrom(true); } }

        public string ClassName { get { return this.Name; } }
    }
}

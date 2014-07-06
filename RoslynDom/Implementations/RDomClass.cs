using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            : base(rawItem, MemberKind.Class, StemMemberKind.Class, members, publicAnnotations)
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

          public override ClassDeclarationSyntax BuildSyntax()
        {
            var modifiers = BuildModfierSyntax();
            //var typeParameters = BuildTypeParameterList();
            //var constraintClauses = BuildConstraintClauses();
            var members = BuildMembers();

            var node = SyntaxFactory.ClassDeclaration(Name)
                            .WithModifiers(modifiers)
                            .WithMembers(members);
            var attributesLists = BuildAttributeListSyntax();
            if (attributesLists.Any()) { node = node.WithAttributeLists(attributesLists); }
            return (ClassDeclarationSyntax)RoslynUtilities.Format(node);
        }

        private SyntaxList<MemberDeclarationSyntax> BuildMembers()
        {
            var list = SyntaxFactory.List<MemberDeclarationSyntax>();
            foreach (var member in Members)
            {
                var memberAsRDomMethod = member as RDomMethod;
                if (memberAsRDomMethod != null)
                { list = list.Add(memberAsRDomMethod.BuildSyntax()); }
            }
            return list;
        }

        public IEnumerable<IClass> Classes
        { get { return Members.OfType<IClass>(); } }

        public IEnumerable<IType> Types
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

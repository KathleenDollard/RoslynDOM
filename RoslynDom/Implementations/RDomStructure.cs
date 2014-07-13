using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomStructureTypeMemberFactory
          : RDomTypeMemberFactory<RDomStructure, StructDeclarationSyntax>
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            // This is identical to Class, but didn't work out reuse here
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var attributeSyntax = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            var node = SyntaxFactory.StructDeclaration(identifier)
                .WithModifiers(modifiers);
            var itemAsStruct = item as IStructure ;
            if (itemAsStruct == null) { throw new InvalidOperationException(); }
            var membersSyntax = itemAsStruct.Members
                        .SelectMany(x => RDomFactoryHelper.TypeMemberFactoryHelper.BuildSyntax(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }


    public class RDomStructureStemMemberFactory
           : RDomStemMemberFactory<RDomStructure, StructDeclarationSyntax>
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            return RDomFactoryHelper.TypeMemberFactoryHelper.BuildSyntax(item);
        }
    }


    public class RDomStructure : RDomBaseType<IStructure, StructDeclarationSyntax>, IStructure
    {
        internal RDomStructure(
                 StructDeclarationSyntax rawItem)
        : base(rawItem, MemberKind.Structure, StemMemberKind.Structure)
        {
            Initialize2();
        }

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

        private void Initialize2()
        {
            Initialize();
            var members = ListUtilities.MakeList(TypedSyntax, x => x.Members, x => RDomFactoryHelper.TypeMemberFactoryHelper.MakeItem(x));
            foreach (var member in members)
            { AddOrMoveMember(member); }
        }

        public override StructDeclarationSyntax BuildSyntax()
        {
            var modifiers = this.BuildModfierSyntax();
            var node = SyntaxFactory.StructDeclaration(Name)
                            .WithModifiers(modifiers);

            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildMembers(true), node, (n, l) => n.WithMembers(l));
            //node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildTypeParameterList(), node, (n, l) => n.WithTypeParameters(l));
            //node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildConstraintClauses(), node, (n, l) => n.WithTypeConstraints(l));
            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildAttributeListSyntax(), node, (n, l) => n.WithAttributeLists(l));

            return (StructDeclarationSyntax)RoslynUtilities.Format(node);
        }


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

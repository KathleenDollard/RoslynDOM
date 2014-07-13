using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
    public class RDomInterfaceTypeMemberFactory
           : RDomTypeMemberFactory<RDomInterface, InterfaceDeclarationSyntax>
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            var modifiers = item.BuildModfierSyntax();
            var identifier = SyntaxFactory.Identifier(item.Name);
            var attributeSyntax = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            var node = SyntaxFactory.InterfaceDeclaration(identifier)
                .WithModifiers(modifiers);
            var itemAsInterface = item as IInterface ;
            if (itemAsInterface == null) { throw new InvalidOperationException(); }
            var membersSyntax = itemAsInterface.Members
                        .SelectMany(x => RDomFactoryHelper.TypeMemberFactoryHelper.BuildSyntax(x))
                        .ToList();
            node = node.WithMembers(SyntaxFactory.List(membersSyntax));
            // TODO: Class type members and type constraints
            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }
    }


    public class RDomInterfaceStemMemberFactory
           : RDomStemMemberFactory<RDomInterface, InterfaceDeclarationSyntax>
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(IStemMember item)
        {
            return RDomFactoryHelper.TypeMemberFactoryHelper.BuildSyntax(item);
        }
    }



    public class RDomInterface : RDomBaseType<IInterface,InterfaceDeclarationSyntax>, IInterface
    {
        internal RDomInterface(
               InterfaceDeclarationSyntax rawItem)
        : base(rawItem, MemberKind.Interface, StemMemberKind.Interface)
        {
            Initialize2();
        }

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

        private void Initialize2()
        {
            Initialize();
            var members = ListUtilities.MakeList(TypedSyntax, x => x.Members, x => RDomFactoryHelper.TypeMemberFactoryHelper.MakeItem(x));
            foreach (var member in members)
            { AddOrMoveMember(member); }
        }

        public override InterfaceDeclarationSyntax BuildSyntax()
        {
            var modifiers = this.BuildModfierSyntax();
            var node = SyntaxFactory.InterfaceDeclaration(Name)
                            .WithModifiers(modifiers);

            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildMembers(false), node, (n, l) => n.WithMembers(l));
            //node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildTypeParameterList(), node, (n, l) => n.WithTypeParameters(l));
            //node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildConstraintClauses(), node, (n, l) => n.WithTypeConstraints(l));
            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildAttributeListSyntax(), node, (n, l) => n.WithAttributeLists(l));

            return (InterfaceDeclarationSyntax)RoslynUtilities.Format(node);
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

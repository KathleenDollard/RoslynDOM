using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomInterface : RDomBaseType<IInterface>, IInterface
    {
        internal RDomInterface(
               SyntaxNode rawItem)
        : base(rawItem, MemberKind.Interface, StemMemberKind.Interface)
        {
            //Initialize2();
        }

        //     internal RDomInterface(
        //    InterfaceDeclarationSyntax rawItem,
        //    IEnumerable<ITypeMember> members,
        //    params PublicAnnotation[] publicAnnotations) 
        //    : base(rawItem, MemberKind.Interface,StemMemberKind.Interface,  members, publicAnnotations )
        //{
        //    Initialize();
        //}

        internal RDomInterface(RDomInterface oldRDom)
             : base(oldRDom)
        { }

        //private void Initialize2()
        //{
        //    Initialize();
        //    var members = ListUtilities.MakeList(TypedSyntax, x => x.Members, x => RDomFactoryHelper.TypeMemberFactoryHelper.MakeItem(x));
        //    foreach (var member in members)
        //    { AddOrMoveMember(member); }
        //}

        //public override InterfaceDeclarationSyntax BuildSyntax()
        //{
        //    var modifiers = this.BuildModfierSyntax();
        //    var node = SyntaxFactory.InterfaceDeclaration(Name)
        //                    .WithModifiers(modifiers);

        //    node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildMembers(false), node, (n, l) => n.WithMembers(l));
        //    //node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildTypeParameterList(), node, (n, l) => n.WithTypeParameters(l));
        //    //node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildConstraintClauses(), node, (n, l) => n.WithTypeConstraints(l));
        //    node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildAttributeListSyntax(), node, (n, l) => n.WithAttributeLists(l));

        //    return (InterfaceDeclarationSyntax)RoslynUtilities.Format(node);
        //}

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

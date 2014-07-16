using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;
using System.Linq;

namespace RoslynDom
{
     public class RDomEnum : RDomBase<IEnum, EnumDeclarationSyntax, ISymbol>, IEnum
    {
        internal RDomEnum(
             EnumDeclarationSyntax rawItem)
           : base(rawItem)
        {
            //Initialize2();
        }

        //internal RDomEnum(
        //  EnumDeclarationSyntax rawItem,
        //  params PublicAnnotation[] publicAnnotations)
        //: base(rawItem, publicAnnotations)
        //{
        //    Initialize();
        //}

        internal RDomEnum(RDomEnum oldRDom)
             : base(oldRDom)
        {
            AccessModifier = oldRDom.AccessModifier;
            UnderlyingType = oldRDom.UnderlyingType;
        }

        //protected override void Initialize()
        //{
        //    base.Initialize();
        //    AccessModifier = GetAccessibility();
        //    var symbol = Symbol as INamedTypeSymbol;
        //    if (symbol != null)
        //    {
        //        var underlyingNamedTypeSymbol = symbol.EnumUnderlyingType;
        //        UnderlyingType = new RDomReferencedType(underlyingNamedTypeSymbol.DeclaringSyntaxReferences, underlyingNamedTypeSymbol);
        //    }
        //}

        //private void Initialize2()
        //{
        //    Initialize();
        //}

        //public override EnumDeclarationSyntax BuildSyntax()
        //{
        //    var modifiers = this.BuildModfierSyntax();
        //    var node = SyntaxFactory.EnumDeclaration(Name)
        //                    .WithModifiers(modifiers);
        //    //var node = SyntaxFactory.EnumDeclaration(Name);
        //    // TODO: Support enum members
        //    node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildAttributeListSyntax(), node, (n, l) => n.WithAttributeLists(l));

        //    return (EnumDeclarationSyntax)RoslynUtilities.Format(node);
        //}

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public string Namespace
        { get { return RoslynDomUtilities.GetNamespace(this.Parent); } }

        public IType ContainingType { get; set; }

        public string QualifiedName
        {
            get { return GetQualifiedName(); }
        }

        public AccessModifier AccessModifier { get; set; }

        public IReferencedType UnderlyingType { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.Enum; }

        }

        public StemMemberKind StemMemberKind
        {
            get { return StemMemberKind.Enum; }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;


namespace RoslynDom
{
    public class RDomEnum : RDomBase<IEnum,EnumDeclarationSyntax, ISymbol>, IEnum
    {
        internal RDomEnum(
            EnumDeclarationSyntax rawItem,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            Initialize();
        }

        internal RDomEnum(RDomEnum oldRDom)
             : base(oldRDom)
        {
            AccessModifier = oldRDom.AccessModifier;
        }

        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = (AccessModifier)Symbol.DeclaredAccessibility;
            var symbol = Symbol as INamedTypeSymbol;
            if (symbol != null)
            {
                var underlyingNamedTypeSymbol = symbol.EnumUnderlyingType;
                UnderlyingType= new RDomReferencedType(underlyingNamedTypeSymbol.DeclaringSyntaxReferences, underlyingNamedTypeSymbol);
            }
        }

        public IEnumerable<IAttribute> Attributes
        {
            get
            { return GetAttributes(); }
        }

        public string Namespace
        {
            get { return GetNamespace(); }
        }

        public string QualifiedName
        {
            get { return GetQualifiedName(); }
        }

        public AccessModifier AccessModifier { get; set; }
        
        public IReferencedType UnderlyingType { get; set; }
       
        public MemberType MemberType
        {
            get { return MemberType.Enum; }

        }

        public StemMemberType StemMemberType
        {
            get { return StemMemberType.Enum; }

        }
    }
}

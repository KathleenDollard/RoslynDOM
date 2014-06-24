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
    public class RDomEnum : RDomSyntaxNodeBase<EnumDeclarationSyntax, ISymbol>, IEnum
    {
        internal RDomEnum(EnumDeclarationSyntax rawItem) : base(rawItem) { }

        public IEnumerable<IAttribute> Attributes
        {
            get
            { return GetAttributes(); }
        }

        public AccessModifier AccessModifier
        {
            get
            {
                Accessibility accessibility = Symbol.DeclaredAccessibility;
                return (AccessModifier)accessibility;
            }
        }

        public IReferencedType UnderlyingType
        {
            get
            {
                var symbol = Symbol as INamedTypeSymbol;
                var underlyingNamedTypeSymbol = symbol.EnumUnderlyingType ;
                var ret = new RDomReferencedType(underlyingNamedTypeSymbol.DeclaringSyntaxReferences, underlyingNamedTypeSymbol);
                return ret;
            }
        }

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

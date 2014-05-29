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
    public class RDomParameter : RDomSyntaxNodeBase<ParameterSyntax, IParameterSymbol>, IParameter
    {
        internal RDomParameter(ParameterSyntax rawItem) : base(rawItem) { }


    public override string QualifiedName
    {
        get
        {
            // TODO: Manage static member's qualified names
            throw new InvalidOperationException("You can't get qualified name for an instance property");
        }
    }

    public IReferencedType Type
    {
        get
        { 
            return new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type);
        }
    }

        public IEnumerable<IAttribute> Attributes
        {
            get
            {
                return this.AttributesFrom();
            }
        }

        public bool IsOut
        {
            get
            {
                return TypedSymbol.RefKind == RefKind.Out;
            }
        }

        public bool IsRef
        {
            get
            {
                return TypedSymbol.RefKind == RefKind.Ref;
            }
        }

        public bool IsParamArray
        {
            get
            {
                return TypedSymbol.IsParams ;
            }
        }

        public bool IsOptional
        {
            get
            {
                return TypedSymbol.IsOptional ;
            }
        }

        public int Ordinal
        {
            get
            {
               return TypedSymbol.Ordinal ;
            }
        }
    }


}

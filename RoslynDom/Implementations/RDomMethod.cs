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
    public class RDomMethod : RDomSyntaxNodeBase<MethodDeclarationSyntax, IMethodSymbol>, IMethod
    {
        private IEnumerable<IParameter> _parameters;
        internal RDomMethod(MethodDeclarationSyntax rawItem, IEnumerable<IParameter> parametrs) : base(rawItem)
        {
            _parameters = parametrs;
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier
        {
            get
            {
                Accessibility accessibility = Symbol.DeclaredAccessibility;
                return (AccessModifier)accessibility;
            }
        }

        public override string QualifiedName
        { get { throw new InvalidOperationException("You can't get qualified name for an instance method"); } }

        public override string Namespace
        { get { throw new InvalidOperationException("You can't get namespace for an instance method"); } }

        public IReferencedType ReturnType
        {
            get
            {
                return new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.ReturnType);
            }
        }

        public bool IsAbstract
        { get { return Symbol.IsAbstract; } }

        public bool IsVirtual
        { get { return Symbol.IsVirtual; } }

        public bool IsOverride
        { get { return Symbol.IsOverride; } }

        public bool IsSealed
        { get { return Symbol.IsSealed; } }

        public bool IsStatic
        { get { return Symbol.IsStatic; } }

        public IEnumerable<ITypeParameter> TypeParameters
        { get { return this.TypedSymbol.TypeParametersFrom(); } }

        public IEnumerable<IParameter> Parameters
        { get { return _parameters; } }

        public bool IsExtensionMethod
        { get { return TypedSymbol.IsExtensionMethod; } }

        public MemberType MemberType
        {
            get { return MemberType.Method; }
        }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            {
                return ReturnType.QualifiedName;
            }
            return base.RequestValue(name);
        }
    }
}

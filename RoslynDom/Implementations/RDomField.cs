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
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// Field assignments in the form "Type x, y, z" are not yet supported 
    /// and when they are they will be loaded as separate fields (rather 
    /// obviously). At that point, the variable declaration will need to be held in
    /// the class. 
    /// </remarks>
    public class RDomField : RDomSyntaxNodeBase<FieldDeclarationSyntax, IFieldSymbol>, IField
    {
        private VariableDeclaratorSyntax _varSyntax;

        internal RDomField(
                    FieldDeclarationSyntax rawItem, 
                    VariableDeclaratorSyntax varSyntax,
                    params PublicAnnotation[] publicAnnotations)
            : base(rawItem, publicAnnotations )
        { _varSyntax = varSyntax; }

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
        private VariableDeclaratorSyntax variableDeclaration
        { get { return _varSyntax; } }

        public override ISymbol Symbol
        { get { return base.GetSymbol(variableDeclaration); } }

        public override IFieldSymbol TypedSymbol
        { get { return (IFieldSymbol)base.GetSymbol(variableDeclaration); } }

        public IReferencedType ReturnType
        { get { return new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type); } }

        public bool IsStatic
        { get { return Symbol.IsStatic; } }

        public MemberType MemberType
        {
            get { return MemberType.Field; }
        }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            {                return ReturnType.QualifiedName;            }
            return base.RequestValue(name);
        }
    }
}

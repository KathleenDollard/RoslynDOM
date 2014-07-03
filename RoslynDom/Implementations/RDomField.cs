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
    public class RDomField : RDomBase<IField, FieldDeclarationSyntax, IFieldSymbol>, IField
    {
        private VariableDeclaratorSyntax _varSyntax;

        internal RDomField(
                    FieldDeclarationSyntax rawItem,
                    VariableDeclaratorSyntax varSyntax,
                    params PublicAnnotation[] publicAnnotations)
            : base(rawItem, publicAnnotations)
        {
            _varSyntax = varSyntax;
            Initialize();
        }

        internal RDomField(RDomField oldRDom)
             : base(oldRDom)
        {
            AccessModifier = oldRDom.AccessModifier;
            ReturnType = oldRDom.ReturnType;
            IsStatic = oldRDom.IsStatic;
        }

        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = (AccessModifier)Symbol.DeclaredAccessibility;
            ReturnType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type);
            IsStatic = Symbol.IsStatic;
        }

        public override bool SameIntent(IField other, bool includePublicAnnotations)
        {
            if (!base.SameIntent(other, includePublicAnnotations)) return false;
            if (AccessModifier != other.AccessModifier) return false;
            if (ReturnType.QualifiedName  != other.ReturnType.QualifiedName ) return false;
            if (IsStatic != other.IsStatic) return false;
            return true;
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }

        private VariableDeclaratorSyntax variableDeclaration
        { get { return _varSyntax; } }

        public override ISymbol Symbol
        { get { return TypedSymbol; } }

        public override IFieldSymbol TypedSymbol
        { get { return (IFieldSymbol)base.GetSymbol(variableDeclaration); } }

        public IReferencedType ReturnType { get; set; }

        public bool IsStatic { get; set; }

        public MemberType MemberType
        {
            get { return MemberType.Field; }
        }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            { return ReturnType.QualifiedName; }
            return base.RequestValue(name);
        }
    }
}

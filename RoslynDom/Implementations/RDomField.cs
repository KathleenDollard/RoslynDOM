using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
            _varSyntax = oldRDom._varSyntax;
            AccessModifier = oldRDom.AccessModifier;
            ReturnType = oldRDom.ReturnType;
            IsStatic = oldRDom.IsStatic;
        }

        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = GetAccessibility();
            ReturnType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type);
            IsStatic = Symbol.IsStatic;
        }

        public override FieldDeclarationSyntax BuildSyntax()
        {
            var nameSyntax = SyntaxFactory.Identifier(Name);
            var returnType = ((RDomReferencedType)ReturnType).BuildSyntax();
            var modifiers = BuildModfierSyntax();
            var declaratorNode = SyntaxFactory.VariableDeclarator(nameSyntax);
            var variableNode = SyntaxFactory.VariableDeclaration(returnType)
               .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(nameSyntax)));
            var node = SyntaxFactory.FieldDeclaration(variableNode )
               .WithModifiers(modifiers);

            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildAttributeListSyntax(), node, (n, list) => n.WithAttributeLists(list));

            return (FieldDeclarationSyntax)RoslynUtilities.Format(node);
        }

        protected override bool CheckSameIntent(IField other, bool includePublicAnnotations)
        {
            if (!base.CheckSameIntent(other, includePublicAnnotations)) return false;
            if (AccessModifier != other.AccessModifier) return false;
            if (ReturnType.QualifiedName != other.ReturnType.QualifiedName) return false;
            if (IsStatic != other.IsStatic) return false;
            return true;
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }

        public override IFieldSymbol TypedSymbol
        { get { return (IFieldSymbol)base.GetSymbol(_varSyntax); } }

        public IReferencedType ReturnType { get; set; }

        public bool IsStatic { get; set; }

        public MemberKind MemberKind
        {
            get { return MemberKind.Field; }
        }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            { return ReturnType.QualifiedName; }
            return base.RequestValue(name);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomMethod : RDomBase<IMethod, MethodDeclarationSyntax, IMethodSymbol>, IMethod
    {
        private IList<IParameter> _parameters = new List<IParameter>();
        private IList<ITypeParameter> _typeParameters = new List<ITypeParameter>();
        private IList<IStatement> _statements = new List<IStatement>();

        internal RDomMethod(
            MethodDeclarationSyntax rawItem,
            IEnumerable<IParameter> parameters,
            IEnumerable<IStatement> statements,
            params PublicAnnotation[] publicAnnotations)
          : base(rawItem, publicAnnotations)
        {
            foreach (var parameter in parameters)
            { AddParameter(parameter); }
            foreach (var statement in statements)
            { AddStatement(statement); }
            Initialize();
        }

        internal RDomMethod(RDomMethod oldRDom)
             : base(oldRDom)
        {
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            foreach (var parameter in newParameters)
            { AddParameter(parameter); }
            var newTypeParameters = RoslynDomUtilities.CopyMembers(oldRDom._typeParameters);
            foreach (var typeParameter in newTypeParameters)
            { AddTypeParameter(typeParameter); }
            var newStatements = RoslynDomUtilities.CopyMembers(oldRDom._statements);
            foreach (var statement in newStatements)
            { AddStatement(statement); }

            AccessModifier = oldRDom.AccessModifier;
            ReturnType = oldRDom.ReturnType;
            IsAbstract = oldRDom.IsAbstract;
            IsVirtual = oldRDom.IsVirtual;
            IsOverride = oldRDom.IsOverride;
            IsSealed = oldRDom.IsSealed;
            IsStatic = oldRDom.IsStatic;
            IsExtensionMethod = oldRDom.IsExtensionMethod;

        }

        protected override void Initialize()
        {
            base.Initialize();

            var typeParameters = this.TypedSymbol.TypeParametersFrom();
            foreach (var typeParameter in typeParameters)
            { AddTypeParameter(typeParameter); }

            AccessModifier = GetAccessibility();
            ReturnType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.ReturnType);
            IsAbstract = Symbol.IsAbstract;
            IsVirtual = Symbol.IsVirtual;
            IsOverride = Symbol.IsOverride;
            IsSealed = Symbol.IsSealed;
            IsStatic = Symbol.IsStatic;
            IsExtensionMethod = TypedSymbol.IsExtensionMethod;
        }

        public override MethodDeclarationSyntax BuildSyntax()
        {
            var nameSyntax = SyntaxFactory.Identifier(Name);
            var returnType = ((RDomReferencedType)ReturnType).BuildSyntax();
            var modifiers = BuildModfierSyntax();
            var node = SyntaxFactory.MethodDeclaration(returnType, nameSyntax)
                            .WithModifiers(modifiers);

            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildAttributeListSyntax(), node, (n, list) => n.WithAttributeLists(list));
            node = RoslynUtilities.UpdateNodeIfItemNotNull (BuildStatementBlock(Statements), node, (n, item) => n.WithBody(item));
            //var parameters = BuildParameterList();
            //var typeParameters = BuildTypeParameterList();
            //var constraintClauses = BuildConstraintClauses();

            return (MethodDeclarationSyntax)RoslynUtilities.Format(node);
        }
  
         public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public AccessModifier AccessModifier { get; set; }
        public IReferencedType ReturnType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsSealed { get; set; }
        public bool IsStatic { get; set; }

        public bool IsExtensionMethod { get; set; }

        public void RemoveTypeParameter(ITypeParameter typeParameter)
        { _typeParameters.Remove(typeParameter); }

        public void AddTypeParameter(ITypeParameter typeParameter)
        { _typeParameters.Add(typeParameter); }

        public IEnumerable<ITypeParameter> TypeParameters
        { get { return _typeParameters; } }

        public void RemoveParameter(IParameter parameter)
        { _parameters.Remove(parameter); }

        public void AddParameter(IParameter parameter)
        { _parameters.Add(parameter); }

        public IEnumerable<IParameter> Parameters
        { get { return _parameters; } }

        public void RemoveStatement(IStatement statement)
        { _statements.Remove(statement); }

        public void AddStatement(IStatement statement)
        { _statements.Add(statement); }

        public IEnumerable<IStatement> Statements
        { get { return _statements ; } }

        public MemberKind MemberKind
        { get { return MemberKind.Method; } }

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

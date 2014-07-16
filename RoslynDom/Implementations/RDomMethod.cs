using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomMethodTypeMemberFactory
          : RDomTypeMemberFactory<RDomMethod, MethodDeclarationSyntax>
    {
        public override void InitializeItem(RDomMethod newItem, MethodDeclarationSyntax rawItem)
        {
            newItem.Name = newItem.TypedSymbol.Name;
            var typeParameters = newItem.TypedSymbol.TypeParametersFrom();
            foreach (var typeParameter in typeParameters)
            { newItem.AddTypeParameter(typeParameter); }

            newItem.AccessModifier = RoslynUtilities.GetAccessibilityFromSymbol(newItem.Symbol);
            newItem.ReturnType = new RDomReferencedType(newItem.TypedSymbol.DeclaringSyntaxReferences, newItem.TypedSymbol.ReturnType);
            newItem.IsAbstract = newItem.Symbol.IsAbstract;
            newItem.IsVirtual = newItem.Symbol.IsVirtual;
            newItem.IsOverride = newItem.Symbol.IsOverride;
            newItem.IsSealed = newItem.Symbol.IsSealed;
            newItem.IsStatic = newItem.Symbol.IsStatic;
            newItem.IsExtensionMethod = newItem.TypedSymbol.IsExtensionMethod;
            var parameters = ListUtilities.MakeList(rawItem, x => x.ParameterList.Parameters, x => RDomFactoryHelper.GetHelper<IMisc>().MakeItem(x));
            foreach (var parameter in parameters)
            { newItem.AddParameter((IParameter)parameter); }
            if (rawItem.Body != null)
            {
                var statements = ListUtilities.MakeList(rawItem, x => x.Body.Statements, x => RDomFactoryHelper.GetHelper<IStatement>().MakeItem(x));
                foreach (var statement in statements)
                { newItem.AddStatement(statement); }
            }
        }

        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            var itemAsMethod = item as IMethod;
            var returnTypeSyntax = (TypeSyntax)RDomFactory.BuildSyntaxGroup(itemAsMethod.ReturnType).First();
            var modifiers = BuildSyntaxExtensions.BuildModfierSyntax(item);
            var node = SyntaxFactory.MethodDeclaration(returnTypeSyntax, nameSyntax)
                            .WithModifiers(modifiers);
            var attributes = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes); }
            var parameterSyntaxList = itemAsMethod.Parameters
                        .SelectMany(x => RDomFactory.BuildSyntaxGroup(x))
                        .OfType<ParameterSyntax>()
                        .ToList();
            if (parameterSyntaxList.Any()) { node = node.WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameterSyntaxList))); }
            node = node.WithBody(RoslynUtilities.MakeStatementBlock(itemAsMethod.Statements));

            // TODO: typeParameters  and constraintClauses 

            return new SyntaxNode[] { RoslynUtilities.Format(node) };
        }

     }


    public class RDomMethod : RDomBase<IMethod, MethodDeclarationSyntax, IMethodSymbol>, IMethod
    {
        private IList<IParameter> _parameters = new List<IParameter>();
        private IList<ITypeParameter> _typeParameters = new List<ITypeParameter>();
        private IList<IStatement> _statements = new List<IStatement>();

        internal RDomMethod(
                MethodDeclarationSyntax rawItem)
           : base(rawItem)
        {
            //Initialize();
        }

        //internal RDomMethod(
        //    MethodDeclarationSyntax rawItem,
        //    IEnumerable<IParameter> parameters,
        //    IEnumerable<IStatement> statements,
        //    params PublicAnnotation[] publicAnnotations)
        //  : base(rawItem, publicAnnotations)
        //{
        //    foreach (var parameter in parameters)
        //    { AddParameter(parameter); }
        //    foreach (var statement in statements)
        //    { AddStatement(statement); }
        //    Initialize();
        //}

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

        //protected override void Initialize()
        //{
        //    base.Initialize();

        //    var typeParameters = this.TypedSymbol.TypeParametersFrom();
        //    foreach (var typeParameter in typeParameters)
        //    { AddTypeParameter(typeParameter); }

        //    AccessModifier = GetAccessibility();
        //    ReturnType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.ReturnType);
        //    IsAbstract = Symbol.IsAbstract;
        //    IsVirtual = Symbol.IsVirtual;
        //    IsOverride = Symbol.IsOverride;
        //    IsSealed = Symbol.IsSealed;
        //    IsStatic = Symbol.IsStatic;
        //    IsExtensionMethod = TypedSymbol.IsExtensionMethod;
        //}

        //private void Initialize2()
        //{
        //    Initialize();
        //    var parameters = ListUtilities.MakeList(TypedSyntax, x => x.ParameterList.Parameters, x => RDomFactoryHelper.MiscFactoryHelper.MakeItem(x));
        //    foreach (var parameter in parameters)
        //    { AddParameter((IParameter)parameter); }
        //    if (TypedSyntax.Body != null)
        //    {
        //        var statements = ListUtilities.MakeList(TypedSyntax, x => x.Body.Statements, x => RDomFactoryHelper.StatementFactoryHelper.MakeItem(x));
        //        foreach (var statement in statements)
        //        { AddStatement(statement); }
        //    }
        //}

        //public override MethodDeclarationSyntax BuildSyntax()
        //{
        //    var nameSyntax = SyntaxFactory.Identifier(Name);
        //    var returnType = ((RDomReferencedType)ReturnType).BuildSyntax();
        //    var modifiers = this.BuildModfierSyntax();
        //    var node = SyntaxFactory.MethodDeclaration(returnType, nameSyntax)
        //                    .WithModifiers(modifiers);

        //    node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildAttributeListSyntax(), node, (n, list) => n.WithAttributeLists(list));
        //    node = RoslynUtilities.UpdateNodeIfItemNotNull(BuildSyntaxExtensions.BuildStatementBlock(Statements), node, (n, item) => n.WithBody(item));
        //    var parameters = BuildParameterList(this);
        //    if (parameters.Any()) { node = node.WithParameterList(SyntaxFactory.ParameterList(parameters)); }
        //    //var typeParameters = BuildTypeParameterList();
        //    //var constraintClauses = BuildConstraintClauses();

        //    return (MethodDeclarationSyntax)RoslynUtilities.Format(node);
        //}

        //private static SeparatedSyntaxList<ParameterSyntax> BuildParameterList(RDomMethod method)
        //{
        //    var list = new List<ParameterSyntax>();
        //    foreach (var parameter in method.Parameters)
        //    {
        //        list.Add(((RDomParameter)parameter).BuildSyntax());
        //    }
        //    return SyntaxFactory.SeparatedList(list);
        //}

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
        { get { return _statements; } }

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

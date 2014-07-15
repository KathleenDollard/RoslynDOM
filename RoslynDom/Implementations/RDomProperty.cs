using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynDom.Common;

namespace RoslynDom
{
    public class RDomPropertyTypeMemberFactory
          : RDomTypeMemberFactory<RDomProperty, PropertyDeclarationSyntax>
    {
        public override IEnumerable<SyntaxNode> BuildSyntax(ITypeMember item)
        {
            var nameSyntax = SyntaxFactory.Identifier(item.Name);
            var itemAsProeprty = item as IProperty;
            var returnType = (TypeSyntax)RDomFactory.BuildSyntax(itemAsProeprty.ReturnType);
            var modifiers = BuildSyntaxExtensions.BuildModfierSyntax(item);
            var node = SyntaxFactory.PropertyDeclaration(returnType, nameSyntax)
                            .WithModifiers(modifiers);

            var attributes = BuildSyntaxExtensions.BuildAttributeListSyntax(item.Attributes);
            if (attributes.Any()) { node = node.WithAttributeLists(attributes); }

            var accessors = SyntaxFactory.List<AccessorDeclarationSyntax>();
            var getAccessorSyntax = RDomFactory.BuildSyntaxGroup(itemAsProeprty.GetAccessor).FirstOrDefault();
            if (getAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)getAccessorSyntax); }
            var setAccessorSyntax = RDomFactory.BuildSyntaxGroup(itemAsProeprty.SetAccessor).FirstOrDefault();
            if (setAccessorSyntax != null) { accessors = accessors.Add((AccessorDeclarationSyntax)setAccessorSyntax); }
            if (accessors.Any()) { node = node.WithAccessorList(SyntaxFactory.AccessorList(accessors)); }
            // TODO: parameters , typeParameters and constraintClauses 

            return new SyntaxNode[] { node.NormalizeWhitespace() };
        }
    }

    public class RDomProperty : RDomBase<IProperty, PropertyDeclarationSyntax, IPropertySymbol>, IProperty
    {
        private IList<IParameter> _parameters = new List<IParameter>();
        private IList<IStatement> _getStatements = new List<IStatement>();
        private IList<IStatement> _setStatements = new List<IStatement>();

        internal RDomProperty(
                PropertyDeclarationSyntax rawItem)
            : base(rawItem)
        {
            Initialize2();
        }

        internal RDomProperty(
             PropertyDeclarationSyntax rawItem,
             IEnumerable<IParameter> parameters,
             IAccessor getAccessor,
             IAccessor setAccessor,
             params PublicAnnotation[] publicAnnotations)
            : base(rawItem, publicAnnotations)
        {
            foreach (var parameter in parameters)
            { AddParameter(parameter); }
            GetAccessor = getAccessor;
            SetAccessor = setAccessor;
            Initialize();
        }

        internal RDomProperty(RDomProperty oldRDom)
            : base(oldRDom)
        {
            var newParameters = RoslynDomUtilities.CopyMembers(oldRDom._parameters);
            foreach (var parameter in newParameters)
            { AddParameter(parameter); }
            AccessModifier = oldRDom.AccessModifier;
            GetAccessor = oldRDom.GetAccessor == null ? null : oldRDom.GetAccessor.Copy();
            SetAccessor = oldRDom.SetAccessor == null ? null : oldRDom.SetAccessor.Copy();
            ReturnType = oldRDom.ReturnType;
            IsAbstract = oldRDom.IsAbstract;
            IsVirtual = oldRDom.IsVirtual;
            IsOverride = oldRDom.IsOverride;
            IsSealed = oldRDom.IsSealed;
            IsStatic = oldRDom.IsStatic;
            CanGet = oldRDom.CanGet;
            CanSet = oldRDom.CanSet;
        }
        protected override void Initialize()
        {
            base.Initialize();
            AccessModifier = GetAccessibility();
            // TODO: Get and set accessibility
            PropertyType = new RDomReferencedType(TypedSymbol.DeclaringSyntaxReferences, TypedSymbol.Type);
            IsAbstract = Symbol.IsAbstract;
            IsVirtual = Symbol.IsVirtual;
            IsOverride = Symbol.IsOverride;
            IsSealed = Symbol.IsSealed;
            IsStatic = Symbol.IsStatic;
            var propSymbol = Symbol as IPropertySymbol;
            if (propSymbol == null) throw new InvalidOperationException();
            CanGet = (!propSymbol.IsWriteOnly); // or check whether getAccessor is null
            CanSet = (!propSymbol.IsReadOnly); // or check whether setAccessor is null
        }

        private void Initialize2()
        {
            Initialize();
            // Parameters are for VB and not supported in C#
            var getAccessorSyntax = TypedSyntax.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.GetAccessorDeclaration).FirstOrDefault();
            var setAccessorSyntax = TypedSyntax.AccessorList.Accessors.Where(x => x.CSharpKind() == SyntaxKind.SetAccessorDeclaration).FirstOrDefault();
            if (getAccessorSyntax != null)
            { GetAccessor = (IAccessor)(RDomFactoryHelper.MiscFactoryHelper.MakeItem(getAccessorSyntax).FirstOrDefault()); }
            if (setAccessorSyntax != null)
            { SetAccessor = (IAccessor)(RDomFactoryHelper.MiscFactoryHelper.MakeItem(setAccessorSyntax).FirstOrDefault()); }
        }

        public override PropertyDeclarationSyntax BuildSyntax()
        {
            var nameSyntax = SyntaxFactory.Identifier(Name);
            var returnType = ((RDomReferencedType)PropertyType).BuildSyntax();
            var modifiers = this.BuildModfierSyntax();
            var node = SyntaxFactory.PropertyDeclaration(returnType, nameSyntax)
                            .WithModifiers(modifiers);

            node = RoslynUtilities.UpdateNodeIfListNotEmpty(BuildAttributeListSyntax(), node, (n, list) => n.WithAttributeLists(list));
            node = RoslynUtilities.UpdateNodeIfItemNotNull(BuildAccessorList(), node, (n, item) => n.WithAccessorList(item));
            //var parameters = BuildTypeParameterList();
            //var typeParameters = BuildTypeParameterList();
            //var constraintClauses = BuildConstraintClauses();

            return (PropertyDeclarationSyntax)RoslynUtilities.Format(node);
        }

        private AccessorListSyntax BuildAccessorList()
        {
            var list = new List<AccessorDeclarationSyntax>();
            if (CanGet)
            {
                var node = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);
                node = RoslynUtilities.UpdateNodeIfItemNotNull(BuildSyntaxExtensions.BuildStatementBlock(GetStatements), node, (n, item) => n.WithBody(item));
                list.Add(node);
            }
            if (CanSet)
            {
                var node = SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration);
                node = RoslynUtilities.UpdateNodeIfItemNotNull(BuildSyntaxExtensions.BuildStatementBlock(SetStatements), node, (n, item) => n.WithBody(item));
                list.Add(node);
            }
            return SyntaxFactory.AccessorList(SyntaxFactory.List<AccessorDeclarationSyntax>(list));
        }

        public IEnumerable<IAttribute> Attributes
        { get { return GetAttributes(); } }

        public IReferencedType PropertyType { get; set; }

        public AccessModifier AccessModifier { get; set; }
        public IAccessor GetAccessor { get; set; }
        public IAccessor SetAccessor { get; set; }

        public IReferencedType ReturnType
        {
            get { return PropertyType; }
            set { PropertyType = value; }
        }

        public bool IsAbstract { get; set; }

        public bool IsVirtual { get; set; }

        public bool IsOverride { get; set; }

        public bool IsSealed { get; set; }

        public bool IsStatic { get; set; }

        public bool CanGet { get; set; }

        public bool CanSet { get; set; }

        public void RemoveParameter(IParameter parameter)
        { _parameters.Remove(parameter); }

        public void AddParameter(IParameter parameter)
        { _parameters.Add(parameter); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This is to support VB, C# does not have parameters on properties. Property parameters
        /// in VB are generally used for indexing, which is managed by "default" in C#
        /// <br/>
        /// Can't test until VB is active
        /// </remarks>
        public IEnumerable<IParameter> Parameters
        // This is for VB, wihch I have not yet implemented, but don't want things crashing so will ignore
        { get { return _parameters; } }

        public void RemoveGetStatement(IStatement statement)
        { _getStatements.Remove(statement); }

        public void AddGetStatement(IStatement statement)
        { _getStatements.Add(statement); }

        public IEnumerable<IStatement> GetStatements
        { get { return _getStatements; } }

        public void RemoveSetStatement(IStatement statement)
        { _setStatements.Remove(statement); }

        public void AddSetStatement(IStatement statement)
        { _setStatements.Add(statement); }

        public IEnumerable<IStatement> SetStatements
        { get { return _setStatements; } }

        public MemberKind MemberKind
        { get { return MemberKind.Property; } }

        public override object RequestValue(string name)
        {
            if (name == "TypeName")
            { return ReturnType.QualifiedName; }
            return base.RequestValue(name);
        }
    }
}
